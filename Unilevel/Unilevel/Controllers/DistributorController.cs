using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Encoders;
using System.Data;
using Unilevel.Data;
using Unilevel.Helpers;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DistributorController : ControllerBase
    {
        public readonly IDistributorRepository _distributorRepository;

        public DistributorController(IDistributorRepository distributorRepository)
        {
            _distributorRepository = distributorRepository;
        }

        // GET: Distributor/List
        [HttpGet("List")]
        public async Task<IActionResult> GetAllDistributor()
        {
            return Ok(await _distributorRepository.GetAllDistributorAsync());
        }

        // POST: Distributor/Create
        [HttpPost("Create")]
        public async Task<IActionResult> AddDistributor(AddDis distributor)
        {
            try
            {
                if (distributor.Name == string.Empty || distributor.Email == string.Empty  || distributor.Address == string.Empty || distributor.PhoneNumber == string.Empty)
                {
                    return BadRequest(new APIRespone(false, "name/email/address/phone number cannot be blank"));
                }
                await _distributorRepository.AddDistributorAsync(distributor);
                return Ok(new APIRespone(true, "success"));
            }
            catch (DuplicateNameException dex)
            {
                return Ok(new APIRespone(false, dex.Message));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // GET: Distributor/List/NotInArea
        [HttpGet("List/NotInArea")]
        public async Task<IActionResult> GetAllDisNotInArea()
        {
            var distributors = await _distributorRepository.GetAllDisNotInAreaAsync();
            return Ok(distributors);
        }

        // PUT: Distributor/AddToArea/{areaCode}/{disId}
        [HttpPut("AddToArea/{areaCode}/{disId}")]
        public async Task<IActionResult> AddDisIntoArea(string areaCode, string disId)
        {
            try
            {
                await _distributorRepository.AddDisIntoAreaAsync(areaCode, disId);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // DELETE: Distributor/RemoveFromArea/{disId}
        [HttpDelete("RemoveFromArea/{disId}")]
        public async Task<IActionResult> RemoveDisFromArea(string disId)
        {
            try
            {
                await _distributorRepository.RemoveDisFromAreaAsync(disId);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }

        // DELETE: Distributor/Delete/{disId}
        [HttpDelete("Delete/{disId}")]
        public async Task<IActionResult> RemoveDistributor(string disId)
        {
            try
            {
                await _distributorRepository.RemoveDistributorAsync(disId);
                return Ok(new APIRespone(true, "success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new APIRespone(false, ex.Message));
            }
        }
    }
}
