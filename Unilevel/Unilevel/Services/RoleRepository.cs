using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class RoleRepository : IRoleRepository
    {
        public readonly UnilevelContext _context;
        public readonly IMapper _mapper;

        public RoleRepository(UnilevelContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RoleDTO>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.ToListAsync();
            return _mapper.Map<List<RoleDTO>>(roles);
        }
    }
}
