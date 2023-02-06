using System.ComponentModel;
using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IJobRepository
    {
        public Task AddJobAsync(AddJob job, string userId);

        public Task<List<CategoryDetail>> GetAllCategoryIdAndNameAsync();

        public Task<List<dynamic>> GetAllVisitIdAndNameAsync();

        public Task RemoveJobAsync(int id, string userId);

        public Task<JobDetails> JobDetailsAsync(int id);

        public Task RatingJobAsync(AddRating rating);

        public Task ChangeStatusJobAsync(int id, int status, string userId);

        public Task<dynamic> EditJobAsync(int id, string userId);

        public Task EditJobAsync(int id, string userId, EditJob job);

        public Task<List<JobSummary>> GetAllMyJobCreateOrAssignAsync(string userId, bool create);

        public Task SendCommentAsync(SendComment comment, string userId);
    }
}
