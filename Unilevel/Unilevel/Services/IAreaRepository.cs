using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IAreaRepository
    {
        public Task<List<AreaInfor>> GetAllAreaAsync();
        public Task<AreaDetail> GetAreaAsync(string areaCode);
        public Task AddAreaAsync(AddOrEditArea area);
        public Task EditAreaAsync(AddOrEditArea area, string areaCode);
        public Task DeleteAreaAsync(string areaCode);
        public Task<List<ViewDis>> GetAllDisNotInAreaAsync();
        public Task AddDisIntoAreaAsync(string areaCode, string disId);
        public Task RemoveDisFromAreaAsync(string disId);
        public Task<List<UserInfo>> GetAllUsersNotInAreaAsync();
        public Task AddUserIntoAreaAsync(string areaCode, string id);
        public Task RemoveUserFromAreaAsync(string id);
    }
}
