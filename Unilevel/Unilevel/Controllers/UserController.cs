using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
using System.Security.Claims;
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
        private readonly IUserService _userService;

        public UserController(IUserRepository userRepository, IUserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }

        // GET: User/List
        [HttpGet("List")]
        [Authorize]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userRepository.GetAllUserAsync();
            return Ok(users);
        }

        // GET: User/List/NotInArea
        [HttpGet("List/NotInArea")]
        [Authorize(Roles = "Owner, Administrator")]
        public async Task<IActionResult> GetAllUserNotInArea()
        {
            var users = await _userRepository.GetAllUsersNotInAreaAsync();
            return Ok(users);
        }

        // POST: User/Create
        [HttpPost("Create")]
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

        // POST: User/Login
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

        [HttpDelete("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = _userService.GetUserId();
            await _userRepository.Logout(userId);
            return Ok();
        }

        // POST: User/RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenModel token)
        {
            try
            {
                var newToken = await _userRepository.RefreshTokenAsync(token);
                return Ok(new {success = true, message = "success", data = newToken});
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // DELETE: User/Delete/{id}
        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Owner, Administrator")]
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

        // PUT: User/AddToArea/{areaCode}/{id}
        [HttpPut("AddToArea/{areaCode}/{id}")]
        [Authorize(Roles = "Owner, Administrator")]
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

        // DELETE: User/RemoveFromArea/{id}
        [HttpDelete("RemoveFromArea/{id}")]
        [Authorize(Roles = "Owner, Administrator")]
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

        // PUT: User/EditInfo
        [HttpPut("EditInfo")]
        [Authorize]
        public async Task<IActionResult> EditInfoUser(EditInfoUser user)
        {
            try
            {
                var userId = User?.Identity?.Name;
                await _userRepository.EditInfoUserAsync(user, userId);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // PUT: User/EditRole/{id}/{roleId}
        [HttpPut("EditRole/{id}/{roleId}")]
        [Authorize(Roles = "Owner, Administrator")]
        public async Task<IActionResult> EditRoleUser(string id, string roleId)
        {
            try
            {
                await _userRepository.EditRoleUserAsync(id, roleId);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // POST: User/ImportFromFileExcel
        [HttpPost("ImportFromFileExcel")]
        [Authorize(Roles = "Owner, Administrator")]
        public async Task<IActionResult> ImportUserFromFileExcel(IFormFile file)
        {
            try
            {
                var list = new List<FileExcelUser>();
                using (var Stream = new MemoryStream())
                {
                    await file.CopyToAsync(Stream);
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(Stream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowcount = worksheet.Dimension.Rows;
                        for (int row = 2; row <= rowcount; row++)
                        {
                            list.Add(new FileExcelUser()
                            {
                                FullName = worksheet.Cells[row, 1].Value.ToString().Trim(),
                                Email = worksheet.Cells[row, 2].Value.ToString().Trim(),
                                Role = worksheet.Cells[row, 3].Value.ToString().Trim(),
                                ReportTo = worksheet.Cells[row, 4].Value.ToString().Trim(),
                            });
                        }
                    }
                }
                await _userRepository.ImportUserFromFileExcelAsync(list);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, $"cannot continue to add this user and the rest of the users because this user is incorrect: { ex.Message }"));
            }
        }

        // PUT: User/ChangePassword
        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePass change)
        {
            try
            {
                if (change.Password == string.Empty || change.NewPassword == string.Empty || change.ConfirmNewPassword == string.Empty)
                {
                    return BadRequest(new APIRespone(false, "invalid password/new password/ confirm new password"));
                }
                if (change.NewPassword != change.ConfirmNewPassword)
                {
                    return BadRequest(new APIRespone(false, "new password and receive new password is not the same"));
                }
                var userId = _userService.GetUserId();
                await _userRepository.ChangePasswordAsync(userId, change.Password, change.NewPassword);
                return Ok(new APIRespone(true, "success"));
            }
            catch(Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }
    }
}
