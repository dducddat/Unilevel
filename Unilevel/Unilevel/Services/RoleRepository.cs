using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
            string id = DateTime.Now.ToString("ddMMyyHHmmss");
            var roleId = await _context.Roles.AnyAsync(r => r.Id == id);
            var roleName = await _context.Roles.AnyAsync(r => r.Name == role.Name && r.Remove == false);
            if (roleId) { throw new Exception("role already exist, please wait a second and recreate"); }
            if (roleName) { throw new DuplicateNameException("name already exist"); }
            _context.Add(new Role { Id = id, Name = role.Name, Remove = false });
            await _context.SaveChangesAsync();
        }

        public async Task AddPermissionOnRoleAsync(AddLinkRoleMenu linkRoleMenu)
        {
            _context.Add(new LinkRoleMenu { RoleId = linkRoleMenu.RoleId, MenuId = linkRoleMenu.MenuId });
            await _context.SaveChangesAsync();
        }

        public async Task DeletePermissionOnRoleAsync(int id)
        {
            var linkRoleMenu = await _context.LinkRoleMenus.SingleOrDefaultAsync(lrm => lrm.Id == id);

            if (linkRoleMenu is null)
                throw new Exception("Not found");

            _context.Remove(linkRoleMenu);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(string roleId)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (role != null)
            {
                role.Remove = true;
                _context.Update(role);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("role not exist");
            }    
        }

        public async Task EditRoleAsync(AddOrEditRole role, string roleId)
        {
            var r = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
            if (r is null) throw new Exception("role not exist");

            var roleName = await _context.Roles.AnyAsync(r => r.Name == role.Name && r.Remove == false);
            if (roleName == false)
            {
                r.Name = role.Name;
                _context.Update(r);
                await _context.SaveChangesAsync();
            }
            else { throw new Exception("name already exist"); }
        }

        public async Task<List<MenuModel>> GetAllMenuAsync()
        {
            var menus = await _context.Menus.ToListAsync();
            return _mapper.Map<List<MenuModel>>(menus);
        }

        public async Task<List<LinkRoleMenuModel>> GetAllPermissionOnRoleAsync()
        {
            var linkRoleMenus = await _context.LinkRoleMenus.Include(lrm => lrm.Role)
                                                            .Include(lrm => lrm.Menu)
                                                            .ToListAsync();

            List<LinkRoleMenuModel> listLinkRoleMenu = new List<LinkRoleMenuModel>();

            foreach (var item in linkRoleMenus)
            {
                listLinkRoleMenu.Add(new LinkRoleMenuModel {
                    Id = item.Id,
                    RoleId = item.RoleId,
                    RoleName = item.Role.Name,
                    MenuId = item.MenuId,
                    MenuPermission = item.Menu.Permission
                });
            }

            return listLinkRoleMenu;
        }

        public async Task<List<RoleDetail>> GetAllRolesAsync()
        {
            var roles = await _context.Roles.Where(r => r.Remove == false).ToListAsync();
            return _mapper.Map<List<RoleDetail>>(roles);
        }
    }
}
