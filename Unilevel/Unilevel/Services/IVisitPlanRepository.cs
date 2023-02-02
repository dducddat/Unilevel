using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IVisitPlanRepository
    {
        public Task<List<string>> AddVisitPlanAsync(VisitPlanAdd visitPlan, string userId);

        public Task<List<dynamic>> GetListDistributorAsync();

        public Task RemoveVisitPlanAsync(int id);

        public Task<VisitPlanDetails> VisitPlanDetailsAsync(int id);

        public Task<List<VisitPlanSummary>> GetAllVisitPlanOfUserAsync(string userId);

    }
}
