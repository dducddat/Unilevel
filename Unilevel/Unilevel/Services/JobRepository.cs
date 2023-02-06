using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class JobRepository : IJobRepository
    {
        private readonly UnilevelContext _context;

        private readonly IMapper _mapper;

        public JobRepository(UnilevelContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddJobAsync(AddJob job, string userId)
        {
            var user = await _context.Users
                .Where(u => u.Email == job.UserEmail)
                .Select(u => new {Id = u.Id})
                .FirstOrDefaultAsync();

            if (user == null)
                throw new Exception("User not found");

            _context.Add(new Notification
            {
                CreateByUserId = userId,
                CreateDate = DateTime.Now,
                Title = "You are asked to do a new task",
                Description = $"You are asked to do a new task: {job.Title}. You can check the task in the task section",
                RecipientUserId = user.Id,
                View = false,
            });
            await _context.SaveChangesAsync();

            _context.Add(new Job
            {
                Title = job.Title,
                Description = job.Description,
                CreateByUserId = userId,
                VisitPlanId = job.VisitPlanId,
                UserId = user.Id,
                CategoryId = job.CategoryId,
                CreateDate = DateTime.Now,
                StartDate = job.StartDate,
                EndDate = job.EndDate,
                Status = 1,
                Remove = false
            });
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusJobAsync(int id, int status, string userId)
        {
            var j = await _context.Jobs.SingleOrDefaultAsync(j => j.Id == id);

            if (j == null) throw new Exception("Job not found");

            if (userId != j.CreateByUserId) throw new Exception("You do not have permission to change status this task");

            j.Status = status;

            _context.Update(j);
            await _context.SaveChangesAsync();
        }

        public async Task<dynamic> EditJobAsync(int id, string userId)
        {
            var job = await _context.Jobs
                .Include(j => j.User)
                .SingleOrDefaultAsync(j => j.Id == id);

            if (job == null) throw new Exception("Job not found");

            if (userId != job.CreateByUserId) throw new Exception("You do not have permission to edit this task");

            return new { Id = job.Id,
                         Title = job.Title,
                         CategoryId = job.CategoryId,
                         UserEmail = job.User.Email,
                         StartDate = job.StartDate,
                         EndDate = job.EndDate,
                         Description = job.Description
                         };
        }

        public async Task EditJobAsync(int id, string userId, EditJob job)
        {
            var j = await _context.Jobs
                .SingleOrDefaultAsync(j => j.Id == job.Id);

            if (j == null) throw new Exception("Job not found");

            if (userId != j.CreateByUserId) throw new Exception("You do not have permission to edit this task");

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == job.UserEmail);

            if (user == null) throw new Exception("User not found");

            if (id != job.Id) throw new Exception("Invalid input");

            j.Title = job.Title;
            j.Description = job.Description;
            j.UserId = user.Id;
            j.StartDate = job.StartDate;
            j.EndDate = job.EndDate;
            j.CategoryId = job.CategoryId;

            _context.Update(j);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryDetail>> GetAllCategoryIdAndNameAsync()
        {
            var categ = await _context.Categories.Where(ctg => ctg.Remove == false).ToListAsync();

            return _mapper.Map<List<CategoryDetail>>(categ);
        }

        public async Task<List<JobSummary>> GetAllMyJobCreateOrAssignAsync(string userId, bool create)
        {
            if (create)
            {
                var job = await _context.Jobs
                .Include(j => j.CreateByUser)
                .Where(j => j.CreateByUserId == userId &&
                            j.Remove == false)
                .ToListAsync();

                return _mapper.Map<List<JobSummary>>(job);
            }
            else
            {
                var job = await _context.Jobs
                .Include(j => j.CreateByUser)
                .Where(j => j.UserId == userId &&
                            j.Remove == false)
                .ToListAsync();

                return _mapper.Map<List<JobSummary>>(job);
            }
        }

        public async Task<List<dynamic>> GetAllVisitIdAndNameAsync()
        {
            var visits = await _context.VisitPlans
                .Where(vp => vp.Remove == false && vp.Status == true)
                .Select(vp => new {Id = vp.Id, Title = vp.Title})
                .ToListAsync();

            List<dynamic> result = new List<dynamic>();
            foreach(var visit in visits)
            {
                result.Add(new { Id = visit.Id, Title = visit.Title });
            }

            return result;
        }

        public async Task<JobDetails> JobDetailsAsync(int id)
        {
            var job = await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.CreateByUser)
                .Include(j => j.User)
                .Where(j => j.Id == id)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    CreateByUser = j.CreateByUser.FullName,
                    User = j.User.FullName,
                    j.CreateDate,
                    j.StartDate,
                    j.EndDate,
                    j.Description,
                    Category = j.Category.Name,
                    j.Status
                })
                .SingleOrDefaultAsync();

            if (job is null)
                throw new Exception("Job not found");

            var comments = await _context.Comments
                .Where(cm => cm.JobId == id)
                .Include(cm => cm.User)
                .Select(cm => new {Avatar = cm.User.Avatar, FullName = cm.User.FullName, Content = cm.Content})
                .ToListAsync();

            string status = string.Empty;

            switch(job.Status)
            {
                case 1:
                    status = "New";
                    break;
                case 3:
                    status = "Resovled";
                    break;
                case 2:
                    status = "Progressing";
                    break;
                case 4:
                    status = "Closed";
                    break;
                case 5:
                    status = "Finished";
                    break;
            }

            var rating = await _context.Ratings.Where(r => r.JobId == id).ToListAsync();

            float resultRating = 0;

            if(rating.Count() > 0)
            {
                float count = 0;

                foreach (var r in rating)
                {
                    count += Convert.ToSingle(r.NumberOfStar);
                }

                resultRating = count/Convert.ToSingle(rating.Count());
            }    

            JobDetails result = new JobDetails();
            result.Id = job.Id;
            result.Title = job.Title;
            result.CreateByUser = job.CreateByUser;
            result.User = job.User;
            result.CreateDate = job.CreateDate.ToString("dd-MM-yyyy HH:mm:ss");
            result.StartDate = job.StartDate;
            result.EndDate = job.EndDate;
            result.Description = job.Description;
            result.Category = job.Category;
            result.Status = status;
            result.Rate = String.Format("{0:0.#}", resultRating);
            result.Comments = new List<CommentSummary>();

            foreach(var comment in comments)
            {
                result.Comments.Add(new CommentSummary {
                    AvatarUser = comment.Avatar,
                    FullNameUser = comment.FullName,
                    Content = comment.Content
                });
            }    

            return result;
        }

        public async Task RatingJobAsync(AddRating rating)
        {
            var job = await _context.Jobs.SingleOrDefaultAsync(j => j.Id == rating.JobId);

            if (job == null)
                throw new Exception("Job not found");

            _context.Add(new Rating { JobId = rating.JobId, NumberOfStar = rating.NumberOfStar});
            await _context.SaveChangesAsync();
        }

        public async Task RemoveJobAsync(int id, string userId)
        {
            var job = await _context.Jobs.SingleOrDefaultAsync(j => j.Id == id);

            if (job is null)
                throw new Exception("Job not found");

            if (userId != job.CreateByUserId) throw new Exception("You do not have permission to delete this task");

            job.Remove = true;
            _context.Update(job);

            await _context.SaveChangesAsync();
        }

        public async Task SendCommentAsync(SendComment comment, string userId)
        {
            var job = _context.Jobs.Any(j => j.Id == comment.JobId);

            if (!job)
                throw new Exception("Job not found");

            _context.Add(new Comment { UserId = userId, CreateDate = DateTime.Now, Content = comment.Content, JobId = comment.JobId});
            await _context.SaveChangesAsync();
        }
    }
}
