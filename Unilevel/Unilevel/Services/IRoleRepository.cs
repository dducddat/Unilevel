using System.Runtime.CompilerServices;
using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IRoleRepository
    {
        public Task<List<RoleDetail>> GetAllRolesAsync();
        public Task AddRoleAsync(AddOrEditRole role);
        public Task EditRoleAsync(AddOrEditRole role, string roleId);
        public Task DeleteRoleAsync(string roleId);
        public Task<List<MenuModel>> GetAllMenuAsync();
        public Task AddPermissionOnRoleAsync(AddLinkRoleMenu linkRoleMenu);
        public Task<List<LinkRoleMenuModel>> GetAllPermissionOnRoleAsync();
        public Task DeletePermissionOnRoleAsync(int id);
    }
}
