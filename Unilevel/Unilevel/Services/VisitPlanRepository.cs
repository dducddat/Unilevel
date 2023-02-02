using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Net;
using Unilevel.Data;
using Unilevel.Models;

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

                if(visitPlan.Time == 3)
                {
                    var visitDateTime = await _context.VisitPlans
                        .Where(vp =>
                            vp.CreateByUserId == userId &&
                            vp.VisitDate.Day == date.Day && 
                            vp.VisitDate.Month == date.Month && 
                            vp.VisitDate.Year == date.Year)
                        .CountAsync();

                    if(visitDateTime != 0)
                    {
                        result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because that day there was a visit in the morning or afternoon or full day hits that day");
                        continue;
                    }    
                }
                else if(visitPlan.Time == 2)
                {
                    var visitDateTimeAllDay = await _context.VisitPlans
                        .Where(vp =>
                            vp.CreateByUserId == userId &&
                            vp.VisitDate.Day == date.Day &&
                            vp.VisitDate.Month == date.Month &&
                            vp.VisitDate.Year == date.Year &&
                            vp.Time == 3)
                        .CountAsync();

                    if (visitDateTimeAllDay >= 1)
                    {
                        result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because there were already full day hits that day");
                        continue;
                    }
                    else
                    {
                        var visitDateTimeAfternoon = await _context.VisitPlans
                            .Where(vp => vp.CreateByUserId == userId &&
                                vp.VisitDate.Day == date.Day &&
                                vp.VisitDate.Month == date.Month &&
                                vp.VisitDate.Year == date.Year &&
                                vp.Time == 2)
                            .CountAsync();

                        if (visitDateTimeAfternoon >= 1)
                        {
                            result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because there was 1 visit that afternoon");
                            continue;
                        }
                    }
                }
                else if(visitPlan.Time == 1)
                {
                    var visitDateTimeAllDay = await _context.VisitPlans
                            .Where(vp => vp.CreateByUserId == userId &&
                                vp.VisitDate.Day == date.Day &&
                                vp.VisitDate.Month == date.Month &&
                                vp.VisitDate.Year == date.Year &&
                                vp.Time == 3)
                            .CountAsync();

                    if (visitDateTimeAllDay >= 1)
                    {
                        result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because there were already full day hits that day");
                        continue;
                    }
                    else
                    {
                        var visitDateTimeMorning = await _context.VisitPlans
                            .Where(vp => vp.CreateByUserId == userId &&
                                vp.VisitDate.Day == date.Day &&
                                vp.VisitDate.Month == date.Month &&
                                vp.VisitDate.Year == date.Year &&
                                vp.Time == 1)
                            .CountAsync();

                        if (visitDateTimeMorning >= 1)
                        {
                            result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because there was 1 visit that morning");
                            continue;
                        }
                    }
                }


                // Check visit date with current date
                if (date < DateTime.Now.AddHours(1))
                {
                    result.Add($"Can't add visit number {count} with date and time equal to {date.ToString("dd-MM-yyyy HH:mm:ss")} because the visit date and time must be one hour or more from the current date");
                    continue;
                }  

                // Check distributor exists or not
                var distributor = _context.Distributors.Any(d => d.Id == visitPlan.DistributorId);

                if (!distributor)
                    throw new Exception("Distributor not found");

                var user = await _context.Users
                    .Where(u => u.Email == visitPlan.Email)
                    .Select(u => new {Id = u.Id})
                    .SingleOrDefaultAsync();

                if (user == null)
                    throw new Exception("User not found");

                // Add
                _context.Add(new VisitPlan
                {
                    Title = visitPlan.Title,
                    Time = visitPlan.Time,
                    CreateByUserId = userId,
                    Purpose = visitPlan.Purpose,
                    CreateDate = DateTime.Now,
                    DistributorId = visitPlan.DistributorId,
                    Remove = false,
                    VisitDate = date,
                    Status = true,
                    GuestId = user.Id
                });
                await _context.SaveChangesAsync();
            }

            return result;
        }

        public async Task<List<VisitPlanSummary>> GetAllVisitPlanOfUserAsync(string userId)
        {
            var visit = await _context.VisitPlans
                .Include(vp => vp.User)
                .Where(vp => vp.CreateByUserId == userId)
                .Where(vp => vp.Remove == false)
                .ToListAsync();

            return _mapper.Map<List<VisitPlanSummary>>(visit);
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

        public async Task RemoveVisitPlanAsync(int id)
        {
            var visit = await _context.VisitPlans.SingleOrDefaultAsync(vp => vp.Id == id);

            if (visit is null)
                throw new Exception("Visit not found");

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
    }
}
