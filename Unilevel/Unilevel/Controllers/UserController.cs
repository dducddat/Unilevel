using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Text;
using MimeKit;
using Unilevel.Helpers;
using Unilevel.Models;
using Unilevel.Services;
using MailKit.Net.Smtp;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserRepository _userRepository;
        public readonly IEmailServices _emailService;

        public UserController(IUserRepository userRepository, IEmailServices emailServices)
        {
            _userRepository = userRepository;
            _emailService= emailServices;
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser (AddUserDTO user)
        {
            try
            {
                await _userRepository.CreateUserAsync(user);
                return Ok( new ObjectRespone (true ,"add new success"));
            }
            catch(Exception ex)
            {
                return BadRequest(new ObjectRespone(true, ex.Message));
            }
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO user) 
        {
            var a = await _userRepository.LoginAsync(user);
            return Ok();
        }

    }
}
