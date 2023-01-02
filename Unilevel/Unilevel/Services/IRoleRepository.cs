using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IRoleRepository
    {
        public Task<List<RoleDTO>> GetAllRolesAsync();
    }
}
