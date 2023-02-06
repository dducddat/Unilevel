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
    [Authorize(Policy = "ManageVisit")]
    public class VisitPlanController : ControllerBase
    {
        private readonly IVisitPlanRepository _visitPlanRepository;

        public VisitPlanController(IVisitPlanRepository visitPlanRepository)
        {
            _visitPlanRepository = visitPlanRepository;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> AddVisitPlan(VisitPlanAdd visitPlan)
        {
            try
            {
                var userId = User.FindFirstValue("id");

                var result = await _visitPlanRepository.AddVisitPlanAsync(visitPlan, userId);

                if(result.Any())
                    return BadRequest(result);

                return Ok(new {Message = "Successful"});
            }
            catch (Exception ex)
            {
                return BadRequest(new {Error = ex.Message});
            }
        }

        [HttpGet("/ListDistributor")]
        public async Task<IActionResult> GetListDistributor()
        {
            return Ok(await _visitPlanRepository.GetListDistributorAsync());
        }

        [HttpGet("/AllVisitPlanOfUserCreated")]
        public async Task<IActionResult> GetAllVisitPlanOfUserCreated()
        {
            var userId = User.FindFirstValue("id");

            return Ok(await _visitPlanRepository.GetAllVisitPlanOfUserCreateOrAssignAsync(userId, true));
        }

        [HttpGet("/AllVisitPlanOfUserAssign")]
        public async Task<IActionResult> GetAllVisitPlanOfUserAssign()
        {
            var userId = User.FindFirstValue("id");

            return Ok(await _visitPlanRepository.GetAllVisitPlanOfUserCreateOrAssignAsync(userId, false));
        }

        [HttpGet("/Details/{id}")]
        public async Task<IActionResult> VisitPlanDetails(int id)
        {
            try
            {
                var result = await _visitPlanRepository.VisitPlanDetailsAsync(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("/Remove/{id}")]
        public async Task<IActionResult> RemoveVisitPlan(int id)
        {
            try
            {
                var userId = User.FindFirstValue("id");

                await _visitPlanRepository.RemoveVisitPlanAsync(id, userId);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("/Confirm/{visitPlanId}")]
        public async Task<IActionResult> ConfirmVisitPlan(int visitPlanId)
        {
            try
            {
                var userId = User.FindFirstValue("id");

                await _visitPlanRepository.ConfirmVisitPlan(userId, visitPlanId);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("/Edit/{visitPlanId}")]
        public async Task<IActionResult> EditVisitPlan(int visitPlanId)
        {
            try
            {
                var userId = User.FindFirstValue("id");

                var result = await _visitPlanRepository.EditVisitPLanAsync(userId, visitPlanId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpPut("/Edit/{visitPlanId}")]
        public async Task<IActionResult> EditVisitPlan(int visitPlanId, EditVisitPlan visitPlan)
        {
            try
            {
                var userId = User.FindFirstValue("id");

                await _visitPlanRepository.EditVisitPLanAsync(userId, visitPlanId, visitPlan);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
