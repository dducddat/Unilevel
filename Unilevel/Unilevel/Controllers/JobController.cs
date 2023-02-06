using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Unilevel.Data;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "ManageTask")]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _job;

        public JobController(IJobRepository job) 
        {
            _job = job;
        }

        [HttpGet("GetAllVisitIdAndName")]
        public async Task<IActionResult> GetAllVisitIdAndName()
        {
            var result = await _job.GetAllVisitIdAndNameAsync();

            return Ok(result);
        }

        [HttpGet("GetAllCategoryIdAndName")]
        public async Task<IActionResult> GetAllCategoryIdAndName()
        {
            var result = await _job.GetAllCategoryIdAndNameAsync();

            return Ok(result);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> AddJob(AddJob job)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                await _job.AddJobAsync(job, userId);

                return Ok(new {Message = "Successful"});
            }
            catch (Exception ex)
            {
                return BadRequest(new {Error = ex.Message});
            }
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> JodDetails(int id)
        {
            try
            {
                return Ok(await _job.JobDetailsAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("Rating")]
        public async Task<IActionResult> RatingJob(AddRating rating)
        {
            try
            {
                await _job.RatingJobAsync(rating);

                return Ok(new { Message = "Successful" });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                await _job.RemoveJobAsync(id, userId);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("GetAllMyJobCreate")]
        public async Task<IActionResult> GetAllMyJobCreate()
        {
            string userId = User.FindFirstValue("id");

            var result = await _job.GetAllMyJobCreateOrAssignAsync(userId, true);

            return Ok(result);
        }

        [HttpGet("GetAllMyJobAssign")]
        public async Task<IActionResult> GetAllMyJobAssign()
        {
            string userId = User.FindFirstValue("id");

            var result = await _job.GetAllMyJobCreateOrAssignAsync(userId, false);

            return Ok(result);
        }

        [HttpGet("EditJob/{id}")]
        public async Task<IActionResult> EditJob(int id)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                var result = await _job.EditJobAsync(id, userId);

                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("EditJob/{id}")]
        public async Task<IActionResult> EditJob(int id, EditJob job)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                await _job.EditJobAsync(id, userId, job);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("ChangeStatus/{id}/{status}")]
        public async Task<IActionResult> ChangeStatus(int id, int status)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                await _job.ChangeStatusJobAsync(id, status, userId);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPost("SendComment")]
        public async Task<IActionResult> SendComment(SendComment comment)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                await _job.SendCommentAsync(comment, userId);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
