using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class RoleRepository : IRoleRepository
    {
        private readonly UnilevelContext _context;
        private readonly IMapper _mapper;

        public RoleRepository(UnilevelContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddRoleAsync(AddOrEditRole role)
        {
            string id = DateTime.Now.ToString("ddMMyyhhmmss");
            var roleId = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (roleId != null) { throw new Exception("role already exist, please wait a second and recreate"); }
            _context.Add(new Role { Id = id, Name = role.Name });
            await _context.SaveChangesAsync();
        }

        public async Task EditRoleAsync(AddOrEditRole role, string roleId)
        {
            var r = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (r != null)
            {
                r.Name = role.Name;
                _context.Update(r);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("role not exist");
            }
        }

        public async Task<List<RoleDetail>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return _mapper.Map<List<RoleDetail>>(roles);
        }
    }
}
