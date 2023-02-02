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
    public class VisitPlanController : ControllerBase
    {
        private readonly IVisitPlanRepository _visitPlanRepository;

        public VisitPlanController(IVisitPlanRepository visitPlanRepository)
        {
            _visitPlanRepository = visitPlanRepository;
        }

        [HttpPost("Create")]
        //[Authorize]
        public async Task<IActionResult> AddVisitPlan(VisitPlanAdd visitPlan)
        {
            try
            {
                var userId = "1101231133214145221";/*User.FindFirstValue("id")*/;

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

        [HttpGet("/AllVisitPlanOfUser")]
        public async Task<IActionResult> GetAllVisitPlanOfUser()
        {
            string userId = "1101231133214145221";

            return Ok(await _visitPlanRepository.GetAllVisitPlanOfUserAsync(userId));
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
                await _visitPlanRepository.RemoveVisitPlanAsync(id);

                return Ok(new { Message = "Successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
