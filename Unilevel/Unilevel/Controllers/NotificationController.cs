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
    [Authorize(Policy = "ManageNotification")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _notification;

        public NotificationController(INotificationRepository notification)
        {
            _notification = notification;
        }

        [HttpPost("Create")]
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
        public async Task<IActionResult> GetAllNotificationCreated()
        {
            var userId = User.FindFirstValue("id");

            return Ok(await _notification.GetAllNotificationCreatedOrReceivedAsync(userId, true));
        }

        [HttpGet("GetAllReceived")]
        public async Task<IActionResult> GetAllNotificationReceived()
        {
            var userId = User.FindFirstValue("id");

            return Ok(await _notification.GetAllNotificationCreatedOrReceivedAsync(userId, false));
        }

        [HttpGet("GetNew")]
        public async Task<IActionResult> NewNotificationReceived()
        {
            var userId = User.FindFirstValue("id");

            return Ok(await _notification.GetNewNotificationReceivedAsync(userId));
        }

        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            try
            {
                return Ok(await _notification.GetNotificationByIdAsync(id, false));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("View/{id}")]
        public async Task<IActionResult> ViewNotificationById(int id)
        {
            try
            {
                return Ok(await _notification.GetNotificationByIdAsync(id, true));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("Remove/{id}")]
        public async Task<IActionResult> RemoveNotification(int id)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                await _notification.RemoveNotificationAsync(id, userId);

                return Ok("Successful");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
