using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "ManageRole")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        // GET: Role/List
        [HttpGet("List")]
        public async Task<IActionResult> GetAllRole()
        {
            var roles = await _roleRepository.GetAllRolesAsync();
            return Ok(roles);
        }

        // POST: Role/Create
        [HttpPost("Create")]
        public async Task<IActionResult> AddRole(AddOrEditRole role)
        {
            try
            {
                if (role.Name != null && role.Name != string.Empty)
                {
                    await _roleRepository.AddRoleAsync(role);
                    return Ok(new { Message = "Successfully added new" });
                } 
                else
                {
                    return Problem(detail: "names cannot be blank", statusCode: StatusCodes.Status400BadRequest);
                }
            }
            catch (DuplicateNameException dex)
            {
                return Problem(detail: dex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Post: Role/AddPermissionOnRole
        [HttpPost("AddPermissionOnRole")]
        public async Task<IActionResult> AddPermissionOnRole(AddLinkRoleMenu linkRoleMenu)
        {
            await _roleRepository.AddPermissionOnRoleAsync(linkRoleMenu);

            return Ok();
        }

        // DELETE: Role/DeletePermissionOnRole/{id}
        [HttpDelete("DeletePermissionOnRole/{id}")]
        public async Task<IActionResult> DeletePermissionOnRole(int id)
        {
            try
            {
                await _roleRepository.DeletePermissionOnRoleAsync(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // GET: Role/ListPermissionOnRole
        [HttpGet("ListPermissionOnRole")]
        public async Task<IActionResult> GetAllPermissionOnRole()
        {
            var linkRoleMenus = await _roleRepository.GetAllPermissionOnRoleAsync();

            return Ok(linkRoleMenus);
        }

        // GET: Role/ListMenu
        [HttpGet("ListMenu")]
        public async Task<IActionResult> GetAllMenu()
        {
            var menus = await _roleRepository.GetAllMenuAsync();

            return Ok(menus);
        }

        // PUT: Role/Edit/{roleId}
        [HttpPut("Edit/{roleId}")]
        public async Task<IActionResult> EditRole(AddOrEditRole role, string roleId)
        {
            try
            {
                if(role.Name != string.Empty && role.Name != null)
                {
                    await _roleRepository.EditRoleAsync(role, roleId);
                    return Ok(new { Message = "Edit successfully" });
                }
                return BadRequest(new { Error = "name connot be blank" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DELETE: Role/Delete/{roleId}
        [HttpDelete("Delete/{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            try
            {
                await _roleRepository.DeleteRoleAsync(roleId);
                return Ok(new { Message = "Successful delete" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
