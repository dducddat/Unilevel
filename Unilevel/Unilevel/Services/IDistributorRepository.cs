using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IDistributorRepository
    {
        public Task<List<DistributorModel>> GetAllDistributorAsync();
        public Task AddDistributorAsync (DistributorModel distributor);
        public Task RemoveDistributorAsync (string distributorId);
    }
}
