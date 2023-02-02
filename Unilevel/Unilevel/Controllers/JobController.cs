using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unilevel.Data;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
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
                string userId = "1101231133214145221";
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
                await _job.RemoveJobAsync(id);

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
            string userId = "1101231133214145221";

            var result = await _job.GetAllMyJobCreateOrAssignAsync(userId, true);

            return Ok(result);
        }

        [HttpGet("GetAllMyJobAssign")]
        public async Task<IActionResult> GetAllMyJobAssign()
        {
            string userId = "1101231133214145221";

            var result = await _job.GetAllMyJobCreateOrAssignAsync(userId, false);

            return Ok(result);
        }

        [HttpGet("EditJob/{id}")]
        public async Task<IActionResult> EditJob(int id)
        {
            var result = await _job.EditJobAsync(id);

            return Ok(result);
        }

        [HttpPut("EditJob/{id}")]
        public async Task<IActionResult> EditJob(int id, EditJob job)
        {
            try
            {
                await _job.EditJobAsync(id, job);

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
                await _job.ChangeStatusJobAsync(id, status);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
