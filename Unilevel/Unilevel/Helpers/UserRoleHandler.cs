using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Unilevel.Data;
using Unilevel.Services;

namespace Unilevel.Helpers
{
    public class UserRoleHandler : AuthorizationHandler<UserRoleRequirement>
    {
        private readonly UnilevelContext _context;
        private readonly IMemoryCache _memoryCache;

        public UserRoleHandler(UnilevelContext context, IMemoryCache memoryCache) {
            _context = context;
            _memoryCache = memoryCache;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRoleRequirement requirement)
        {
            if (!_memoryCache.TryGetValue("LinkRoleMenu", out List<LinkRoleMenu> listLinkRoleMenu))
            {
                listLinkRoleMenu = _context.LinkRoleMenus.ToList();

                var cacheExpiryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddHours(1),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(55)
                };

                _memoryCache.Set("LinkRoleMenu", listLinkRoleMenu, cacheExpiryOption);
            }

            if (!_memoryCache.TryGetValue("Menu", out List<Menu> listMenu))
            {
                listMenu = _context.Menus.ToList();

                var cacheExpiryOption = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(30),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(25)
                };

                _memoryCache.Set("Menu", listMenu, cacheExpiryOption);
            }

            var roleId = context.User.FindFirstValue(ClaimTypes.Role);

            var ListPermission = listLinkRoleMenu.Where(lrm => lrm.RoleId == roleId)
                                             .Join(listMenu, 
                                             lrm => lrm.MenuId, m => m.Id, (lrm, m) => new {Permission = m.Permission})
                                             .ToList();

            foreach (var permission in ListPermission)
            {
                if (permission.Permission == requirement.Permission)
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
