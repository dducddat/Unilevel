using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Unilevel.Helpers;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: User/User-List
        [HttpGet("User-List")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userRepository.GetAllUserAsync();
            return Ok(users);
        }

        // GET: User/Users-Not-In-Area
        [HttpGet("Users-Not-In-Area")]
        public async Task<IActionResult> GetAllUserNotInArea()
        {
            var users = await _userRepository.GetAllUsersNotInAreaAsync();
            return Ok(users);
        }

        // POST: User/Create-User
        [HttpPost("Create-User")]
        public async Task<IActionResult> CreateUser(AddUser user)
        {
            try
            {
                await _userRepository.CreateUserAsync(user);
                return Ok(new APIRespone(true, "success"));

            }
            catch (DuplicateNameException nex)
            {
                return BadRequest(new APIRespone(false, nex.Message));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLogin user)
        {
            try
            {
                var token = await _userRepository.LoginAsync(user);
                return Ok(new { success = true, message = "logged in successfully", data = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // DELETE: User/Delete-User/{id}
        [HttpDelete("Delete-User/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userRepository.DeleteUserAsync(id);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // PUT: User/Add-User-Into-Area/{areaCode}/{id}
        [HttpPut("Add-User-Into-Area/{areaCode}/{id}")]
        public async Task<IActionResult> AddUserIntoArea(string areaCode, string id)
        {
            try
            {
                await _userRepository.AddUserIntoAreaAsync(areaCode, id);
                return Ok(new APIRespone(true, "success"));
            }
            catch(Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // DELETE: User/Delete-User-From-Area/{id}
        [HttpDelete("Delete-User-From-Area/{id}")]
        public async Task<IActionResult> RemoveUserFromArea(string id)
        {
            try
            {
                await _userRepository.RemoveUserFromAreaAsync(id);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // PUT: User/Edit-Info-User/{id}
        [HttpPut("Edit-Info-User/{id}")]
        public async Task<IActionResult> EditInfoUser(EditInfoUser user, string id)
        {
            try
            {
                await _userRepository.EditInfoUserAsync(user, id);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        [HttpPut("Edit-Role-User/{id}/{roleCode}")]
        public async Task<IActionResult> EditRoleUser(string id, string roleCode)
        {
            try
            {
                await _userRepository.EditRoleUserAsync(id, roleCode);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }
    }
}
