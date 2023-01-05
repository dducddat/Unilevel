using System.Runtime.CompilerServices;
using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IRoleRepository
    {
        public Task<List<RoleDetail>> GetAllRolesAsync();
        public Task AddRoleAsync(AddOrEditRole role);
        public Task EditRoleAsync(AddOrEditRole role, string roleId);
    }
}
