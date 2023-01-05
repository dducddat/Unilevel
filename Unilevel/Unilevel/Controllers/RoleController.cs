using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unilevel.Data;
using Unilevel.Helpers;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // GET: Role/Role-List
        [HttpGet("Role-List")]
        [Authorize]
        public async Task<IActionResult> GetAllRole()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return Ok(roles);
        }

        // POST: Role/Create-Role
        [HttpPost("Create-Role")]
        [Authorize]
        public async Task<IActionResult> AddRole(AddOrEditRole role)
        {
            try
            {
                if (role.Name != null && role.Name != string.Empty)
                {
                    await _roleRepository.AddRoleAsync(role);
                    return Ok(new APIRespone(true, "success"));
                } 
                else
                {
                    return Problem(detail: "names cannot be blank", statusCode: StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // POST: Role/Edit-Role/{roleId}
        [HttpPut("Edit-Role/{roleId}")]
        public async Task<IActionResult> EditRole(AddOrEditRole role, string roleId)
        {
            try
            {
                if(role.Name != string.Empty && role.Name != null)
                {
                    await _roleRepository.EditRoleAsync(role, roleId);
                    return Ok(new APIRespone(true, "success"));
                }
                return BadRequest(new APIRespone(false, "name connot be blank"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }
    }
}
