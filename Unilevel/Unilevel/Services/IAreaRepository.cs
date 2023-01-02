using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IAreaRepository
    {
        Task<List<AreaDisQtyDTO>> GetAllAreaAsync();
        //public Task<AreaDetailDTO> GetAreaAsync(int id);
        public Task AddAreaAsync(AreaNameDTO name);
        public Task EditAreaAsync(AreaNameDTO name, string areaCode);
        //public Task DeleteAreaAsync(int id);
    }
}
