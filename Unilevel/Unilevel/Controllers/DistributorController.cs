using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using System.Data;
using Unilevel.Models;
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

        //[HttpPost("Add-Distributor")]
        //public async Task<IActionResult> AddDistributor(DistributorModel distributor)
        //{
        //    try
        //    {
        //        await _distributorRepository.AddDistributorAsync(distributor);
        //    }
        //    catch (DuplicateNameException dex)
        //    {

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
    }
}
