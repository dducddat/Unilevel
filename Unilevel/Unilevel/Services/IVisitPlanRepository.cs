using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IVisitPlanRepository
    {
        public Task<List<string>> AddVisitPlanAsync(VisitPlanAdd visitPlan, string userId);

        public Task<List<dynamic>> GetListDistributorAsync();

        public Task RemoveVisitPlanAsync(int id, string userId);

        public Task<VisitPlanDetails> VisitPlanDetailsAsync(int id);

        public Task<List<VisitPlanSummary>> GetAllVisitPlanOfUserCreateOrAssignAsync(string userId, bool created);

        public Task ConfirmVisitPlan(string userId, int visitPlanId);

        public Task<object> EditVisitPLanAsync(string userId, int visitPlanId);

        public Task EditVisitPLanAsync(string userId, int visitPlanId, EditVisitPlan visitPlan);
    }
}
