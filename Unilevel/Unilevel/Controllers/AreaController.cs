using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]/[Action]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaRepository _areaRepo;

        public AreaController(IAreaRepository areaRepo)
        {
            _areaRepo = areaRepo;
        }

        // GET: Area/GetAllArea
        [HttpGet]
        public async Task<IActionResult> GetAllArea()
        {
            return Ok(await _areaRepo.GetAllAreaAsync());
        }

        // GET: Area/GetAreaById/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAreaById(int id)
        {
            try
            {
                var area = await _areaRepo.GetAreaAsync(id);
                return Ok(area);
            }
            catch
            {
                return NotFound();
            }
        }

        // POST: Area/AddArea
        [HttpPost]
        public async Task<IActionResult> AddArea(AddArea areaName)
        {
            if (areaName.AreaName != null)
            {
                await _areaRepo.AddAreaAsync(areaName);
                return Ok();
            }
            return BadRequest();
        }
    }
}