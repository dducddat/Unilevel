﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Policy = "ManageDistributor")]
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
                    return BadRequest(new { Error = "name/email/address/phone number cannot be blank" });
                }
                await _distributorRepository.AddDistributorAsync(distributor);
                return Ok(new {Message = "Successfully added new" });
            }
            catch (DuplicateNameException dex)
            {
                return Ok(new { Error = dex.Message });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // DELETE: Distributor/Delete/{disId}
        [HttpDelete("Delete/{disId}")]
        public async Task<IActionResult> RemoveDistributor(string disId)
        {
            try
            {
                await _distributorRepository.RemoveDistributorAsync(disId);
                return Ok(new {Message = "Successful delete" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
