using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IDistributorRepository
    {
        public Task<List<ViewDis>> GetAllDistributorAsync();
        public Task AddDistributorAsync (AddDis distributor);
        public Task RemoveDistributorAsync (string distributorId);
    }
}
