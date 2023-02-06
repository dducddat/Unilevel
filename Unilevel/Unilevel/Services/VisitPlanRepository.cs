using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.ComponentModel;
using System.Net;
using Unilevel.Data;
using Unilevel.Jobs;
using Unilevel.Models;
using static Quartz.Logging.OperationName;

namespace Unilevel.Services
{
    public class VisitPlanRepository : IVisitPlanRepository
    {
        private readonly UnilevelContext _context;

        private readonly IMapper _mapper;

        public VisitPlanRepository(UnilevelContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<string>> AddVisitPlanAsync(VisitPlanAdd visitPlan, string userId)
        {
            List<string> result = new List<string>();
            int count = 0;

            foreach (var date in visitPlan.VisitDate)
            {
                count++;

                // Check visit date with current date
                if (date < DateTime.Now.AddHours(1))
                {
                    result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because the visit date and time must be one hour or more from the current date");
                    continue;
                }

                if (visitPlan.Time == 3)
                {
                    var visitDateTime = await CheckVisitDate(0, date, userId);

                    if (visitDateTime != 0)
                    {
                        result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because that day there was a visit in the morning or afternoon or full day hits that day");
                        continue;
                    }
                }
                else if (visitPlan.Time == 2)
                {
                    var visitDateTimeAllDay = await CheckVisitDate(3, date, userId);

                    if (visitDateTimeAllDay >= 1)
                    {
                        result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because there were already full day hits that day");
                        continue;
                    }
                    else
                    {
                        var visitDateTimeAfternoon = await CheckVisitDate(2, date, userId);

                        if (visitDateTimeAfternoon >= 1)
                        {
                            result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because there was 1 visit that afternoon");
                            continue;
                        }
                    }
                }
                else if (visitPlan.Time == 1)
                {
                    var visitDateTimeAllDay = await CheckVisitDate(3, date, userId);

                    if (visitDateTimeAllDay >= 1)
                    {
                        result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because there were already full day hits that day");
                        continue;
                    }
                    else
                    {
                        var visitDateTimeMorning = await CheckVisitDate(1, date, userId);

                        if (visitDateTimeMorning >= 1)
                        {
                            result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because there was 1 visit that morning");
                            continue;
                        }
                    }
                }

                // Check distributor exists or not
                var distributor = _context.Distributors.Any(d => d.Id == visitPlan.DistributorId);

                if (!distributor)
                    throw new Exception("Distributor not found");

                var user = await _context.Users
                    .Where(u => u.Email == visitPlan.Email)
                    .Select(u => new { Id = u.Id })
                    .SingleOrDefaultAsync();

                if (user == null)
                    throw new Exception("User not found");

                VisitPlan visit = new VisitPlan
                {
                    Title = visitPlan.Title,
                    Time = visitPlan.Time,
                    CreateByUserId = userId,
                    Purpose = visitPlan.Purpose,
                    CreateDate = DateTime.Now,
                    DistributorId = visitPlan.DistributorId,
                    Remove = false,
                    VisitDate = date,
                    Status = false,
                    GuestId = user.Id
                };

                // Add
                _context.Add(visit);
                await _context.SaveChangesAsync();

                _context.Add(new Notification
                {
                    CreateByUserId = userId,
                    CreateDate = DateTime.Now,
                    Title = "You have been invited to visit",
                    Description = $"You have been invited to visit: {visit.Title}. Please check in the visit plan and confirm this visit plan",
                    RecipientUserId = user.Id,
                    View = false,
                });
                await _context.SaveChangesAsync();
            }

            return result;
        }

        private async Task<int> CheckVisitDate(int time, DateTime date, string userId)
        {
            if(time != 0)
            {
                return await _context.VisitPlans
                                .Where(vp =>
                                    vp.CreateByUserId == userId &&
                                    vp.VisitDate.Day == date.Day &&
                                    vp.VisitDate.Month == date.Month &&
                                    vp.VisitDate.Year == date.Year &&
                                    vp.Time == time &&
                                    vp.Remove == false)
                                .CountAsync();
            }
            else
            {
                return await _context.VisitPlans
                        .Where(vp =>
                            vp.CreateByUserId == userId &&
                            vp.VisitDate.Day == date.Day &&
                            vp.VisitDate.Month == date.Month &&
                            vp.VisitDate.Year == date.Year &&
                            vp.Remove == false)
                        .CountAsync();
            }
        }

        public async Task<List<VisitPlanSummary>> GetAllVisitPlanOfUserCreateOrAssignAsync(string userId, bool created)
        {
            if(created)
            {
                var visit = await _context.VisitPlans
                .Include(vp => vp.User)
                .Where(vp => vp.CreateByUserId == userId)
                .Where(vp => vp.Remove == false)
                .ToListAsync();

                return _mapper.Map<List<VisitPlanSummary>>(visit);
            }
            else
            {
                var visit = await _context.VisitPlans
                .Include(vp => vp.User)
                .Where(vp => vp.GuestId == userId)
                .Where(vp => vp.Remove == false)
                .ToListAsync();

                return _mapper.Map<List<VisitPlanSummary>>(visit);
            } 
            
        }

        public async Task<List<dynamic>> GetListDistributorAsync()
        {
            var distributors = await _context.Distributors
                .Where(dis => dis.Remove == false)
                .Select(dis => new {Id = dis.Id, Name = dis.Name, Email = dis.Email})
                .ToListAsync();

            List<dynamic> result = new List<dynamic>();

            foreach(var distributor in distributors)
            {
                result.Add(new { Id = distributor.Id, Name = distributor.Name, Email = distributor.Email });
            }

            return result;
        }

        public async Task RemoveVisitPlanAsync(int id, string userId)
        {
            var visit = await _context.VisitPlans.SingleOrDefaultAsync(vp => vp.Id == id);

            if (visit is null)
                throw new Exception("Visit not found");

            if (userId != visit.CreateByUserId)
                throw new Exception("You do not have permission to delete this visit plan");    

            visit.Remove = true;

            _context.Update(visit);
            await _context.SaveChangesAsync();
        }

        public async Task<VisitPlanDetails> VisitPlanDetailsAsync(int id)
        {
            var visit = await _context.VisitPlans
                .Include(vp => vp.Guest)
                .Where(vp => vp.Id == id)
                .Where(vp => vp.Remove == false)
                .SingleOrDefaultAsync();

            if (visit is null)
                throw new Exception("Visit not found");

            var job = await _context.Jobs.Include(j => j.CreateByUser).Where(j => j.VisitPlanId == id).ToListAsync();

            string status = visit.Status ? "Visited" : "Not Visited";

            return new VisitPlanDetails {
                Id = visit.Id,
                Title = visit.Title,
                CreateDate = visit.CreateDate.ToString("dd-MM-yyyy HH:mm:ss"),
                Guest = visit.Guest.FullName,
                Status = status,
                Purpose = visit.Purpose,
                ListJobSummary = _mapper.Map<List<JobSummary>>(job)
            };
        }

        public async Task ConfirmVisitPlan(string userId, int visitPlanId)
        {
            var visit = await _context.VisitPlans.Where(vp => vp.Id == visitPlanId && vp.GuestId == userId).SingleOrDefaultAsync();

            if (visit is null) throw new Exception("Visit plan not found");

            visit.Status = true;

            _context.Update(visit);

            await _context.SaveChangesAsync();
        }

        public async Task<object> EditVisitPLanAsync(string userId, int visitPlanId)
        {
            var visit = await _context.VisitPlans
                .Where(vp => vp.Id == visitPlanId && vp.Remove == false)
                .SingleOrDefaultAsync();

            if (visit is null) throw new Exception("Visit not found");

            if (userId != visit.CreateByUserId) throw new Exception("You do not have permission to edit this visit plan");

            return new { Id = visit.Id, Title = visit.Title, VisitDate = visit.VisitDate, Time = visit.Time, Distributor = visit.DistributorId, Purpose = visit.Purpose};
        }

        public async Task EditVisitPLanAsync(string userId, int visitPlanId, EditVisitPlan visitPlan)
        {
            var visit = await _context.VisitPlans
                .Where(vp => vp.Id == visitPlanId && vp.Remove == false)
                .SingleOrDefaultAsync();

            if (visit is null) throw new Exception("Visit not found");

            if (userId != visit.CreateByUserId) throw new Exception("You do not have permission to edit this visit plan");

            if (visitPlanId != visitPlan.Id) throw new Exception("Invalid");

            if (visitPlan.VisitDate < DateTime.Now.AddHours(1))
            {
                throw new Exception($"Can't edit visit with date and time equal to {visitPlan.VisitDate.ToString("dd-MM-yyyy HH:mm:ss")} because the visit date and time must be one hour or more from the current date");
            }

            if (visitPlan.Time == 3)
            {
                var visitDateTime = await CheckVisitDate(0, visitPlan.VisitDate, userId);

                if (visitDateTime != 0)
                {
                    throw new Exception($"Can't edit visit with date and time equal to {visitPlan.VisitDate.ToString("dd-MM-yyyy HH:mm:ss")} because that day there was a visit in the morning or afternoon or full day hits that day");
                }
            }
            else if (visitPlan.Time == 2)
            {
                var visitDateTimeAllDay = await CheckVisitDate(3, visitPlan.VisitDate, userId);

                if (visitDateTimeAllDay >= 1)
                {
                    throw new Exception($"Can't edit visit with date and time equal to {visitPlan.VisitDate.ToString("dd-MM-yyyy HH:mm:ss")} because there were already full day hits that day");
                }
                else
                {
                    var visitDateTimeAfternoon = await CheckVisitDate(2, visitPlan.VisitDate, userId);

                    if (visitDateTimeAfternoon >= 1)
                    {
                        throw new Exception($"Can't edit visit with date and time equal to {visitPlan.VisitDate.ToString("dd-MM-yyyy HH:mm:ss")} because there was 1 visit that afternoon");
                    }
                }
            }
            else if (visitPlan.Time == 1)
            {
                var visitDateTimeAllDay = await CheckVisitDate(3, visitPlan.VisitDate, userId); ;

                if (visitDateTimeAllDay >= 1)
                {
                    throw new Exception($"Can't edit visit with date and time equal to {visitPlan.VisitDate.ToString("dd-MM-yyyy HH:mm:ss")} because there were already full day hits that day");
                }
                else
                {
                    var visitDateTimeMorning = await CheckVisitDate(1, visitPlan.VisitDate, userId);

                    if (visitDateTimeMorning >= 1)
                    {
                        throw new Exception($"Can't edit visit with date and time equal to {visitPlan.VisitDate.ToString("dd-MM-yyyy HH:mm:ss")} because there was 1 visit that morning");
                    }
                }
            }

            var distributor = _context.Distributors.Any(d => d.Id == visitPlan.DistributorId);

            if (!distributor)
                throw new Exception("Distributor not found");

            visit.VisitDate = visitPlan.VisitDate;
            visit.Title = visitPlan.Title;
            visit.Time = visitPlan.Time;
            visit.Purpose = visitPlan.Purpose;
            visit.DistributorId = visitPlan.DistributorId;

            _context.Update(visit);
            await _context.SaveChangesAsync();
        }
    }
}
