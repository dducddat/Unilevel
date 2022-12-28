using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class DistributorController : ControllerBase
    {
        public readonly IDistributorRepository _distributorRepository;

        public DistributorController(IDistributorRepository distributorRepository)
        {
            _distributorRepository = distributorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDistributor()
        {
            return Ok(await _distributorRepository.GetAllDistributorAsync());
        }
    }
}
