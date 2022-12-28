using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IDistributorRepository
    {
        public Task<List<DistributorModel>> GetAllDistributorAsync();
    }
}
