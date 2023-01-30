using Microsoft.AspNetCore.Authorization;
using System.Security.Policy;

namespace Unilevel.Helpers
{
    public class UserRoleRequirement : IAuthorizationRequirement
    {
        public string Permission { get; set; }
        public UserRoleRequirement(string permission) 
        {
            Permission = permission;
        }
    }
}
