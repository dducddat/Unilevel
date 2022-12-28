using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IAreaRepository
    {
        public Task<List<AreaList>> GetAllAreaAsync();
        public Task<AreaDetail> GetAreaAsync(int id);
        public Task AddAreaAsync(AddArea areaName);
    }
}
