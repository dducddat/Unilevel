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

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
            return Ok(new a1(a));
        }
    }

    public class a1
    {
        public byte[] data { get; set; }

        public a1(byte[] data1) { data = data1 ?? new byte[0]; }
    }
}
