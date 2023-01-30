using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Data;
using System.Security.Claims;
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

        // GET: User/List
        [HttpGet("List")]
        [Authorize(Policy = "CanManageUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var users = await _userRepository.GetAllUserAsync();
            return Ok(users);
        }

        // POST: User/Create
        [HttpPost("Create")]
        [Authorize(Policy = "CanManageUser")]
        public async Task<IActionResult> CreateUser(AddUser user)
        {
            try
            {
                await _userRepository.CreateUserAsync(user);
                return Ok(new { Message = "success" });

            }
            catch (DuplicateNameException nex)
            {
                return BadRequest(new { Error = nex.Message });
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
                return Ok(new { Message = "logged in successfully", JWT = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("Logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue("id");
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
                return Ok(new { Message = "success", JWT = newToken});
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DELETE: User/Delete/{id}
        [HttpDelete("Delete/{id}")]
        [Authorize(Policy = "CanManageUser")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                await _userRepository.DeleteUserAsync(id);
                return Ok(new { Message = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // PUT: User/EditInfo
        [HttpPut("EditInfo")]
        [Authorize]
        public async Task<IActionResult> EditInfoUser(EditInfoUser user)
        {
            try
            {
                var userId = User.FindFirstValue("id");
                await _userRepository.EditInfoUserAsync(user, userId);
                return Ok(new { Message = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // PUT: User/EditRole/{id}/{roleId}
        [HttpPut("EditRole/{id}/{roleId}")]
        [Authorize(Policy = "CanManageUser")]
        public async Task<IActionResult> EditRoleUser(string id, string roleId)
        {
            try
            {
                await _userRepository.EditRoleUserAsync(id, roleId);
                return Ok(new { Message = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // POST: User/ImportFromFileExcel
        [HttpPost("ImportFromFileExcel")]
        [Authorize(Policy = "CanManageUser")]
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
                return Ok(new { Message = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = $"cannot continue to add this user and the rest of the users because this user is incorrect: {ex.Message}" });
            }
        }

        // PUT: User/ChangePassword
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePass change)
        {
            try
            {
                if (change.Password == string.Empty || change.NewPassword == string.Empty || change.ConfirmNewPassword == string.Empty)
                {
                    return BadRequest(new { Error = "invalid password/new password/ confirm new password" });
                }
                if (change.NewPassword != change.ConfirmNewPassword)
                {
                    return BadRequest(new { Error = "new password and receive new password is not the same" });
                }
                var userId = User.FindFirstValue("id");
                await _userRepository.ChangePasswordAsync(userId, change.Password, change.NewPassword);
                return Ok(new { Message = "success" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // GET: User/ListSurvey
        [HttpGet("ListSurvey")]
        [Authorize]
        public async Task<IActionResult> GetAllSurveyOfUserId()
        {
            try
            {
                string userId = User.FindFirstValue("id");
                var surveys = await _userRepository.GetAllSurveyOfUserIdAsync(userId);
                return Ok(surveys);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // GET: User/QuestionOfSurvey/{surveyId}
        [HttpGet("QuestionOfSurvey/{surveyId}")]
        public async Task<IActionResult> GetAllQuestionBySurveyId(string surveyId)
        {
            var listQuestion = await _userRepository.GetAllQuestionBySurveyIdAsync(surveyId);

            return Ok(listQuestion);
        }

        // POST: User/UserSendResultSurvey
        [HttpPost("UserSendResultSurvey")]
        [Authorize]
        public async Task<IActionResult> UserSendResultSurvey(ResultSurveyModel resultSurvey)
        {
            try
            {
                string userId = User.FindFirstValue("id");
                await _userRepository.UserSendResultSurveyAsync(resultSurvey, userId);
                return Ok(new { Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
