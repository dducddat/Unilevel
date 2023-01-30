using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notification;

        public NotificationController(INotificationRepository notification)
        {
            _notification = notification;
        }

        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> AddNotification(AddNotification notification)
        {
            if (notification.Title == string.Empty || notification.Description == string.Empty)
                return BadRequest("Invalid input");

            var userId = User.FindFirstValue("id");

            await _notification.AddNotificationAsync(notification, userId);

            return Ok("Successful");
        }

        [HttpGet("GetListUser")]
        public async Task<IActionResult> GetListUser()
        {
            return Ok(await _notification.GetListUserAsync());
        }

        [HttpGet("GetAllCreated")]
        [Authorize]
        public async Task<IActionResult> GetAllNotificationCreated()
        {
            var userId = User.FindFirstValue("id");

            return Ok(await _notification.GetAllNotificationCreatedAsync(userId));
        }

        [HttpGet("GetAllReceived")]
        [Authorize]
        public async Task<IActionResult> GetAllNotificationReceived()
        {
            var userId = User.FindFirstValue("id");

            return Ok(await _notification.GetAllNotificationReceivedAsync(userId));
        }
    }
}
