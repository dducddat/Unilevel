using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IAreaRepository
    {
        public Task<List<AreaInfor>> GetAllAreaAsync();
        //public Task<AreaDetailDTO> GetAreaAsync(int id);
        public Task AddAreaAsync(AddOrEditArea area);
        public Task EditAreaAsync(AddOrEditArea area, string areaCode);
        public Task DeleteAreaAsync(string areaCode);
    }
}
