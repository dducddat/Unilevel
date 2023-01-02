using Microsoft.AspNetCore.Mvc;
using Unilevel.Helpers;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaRepository _areaRepo;

        public AreaController(IAreaRepository areaRepo)
        {
            _areaRepo = areaRepo;
        }

        //GET: Area/ListArea
        [HttpGet("ListArea")]
         public async Task<IActionResult> GetAllArea()
        {
            return Ok(await _areaRepo.GetAllAreaAsync());
        }

        // GET: Area/GetAreaById/id
        //[HttpGet("GetAreaById/{id}")]
        // public async Task<IActionResult> GetAreaById(int id)
        // {
        //     try
        //     {
        //         var area = await _areaRepo.GetAreaAsync(id);
        //         return Ok(area);
        //     }
        //     catch (Exception ex)
        //     {
        //         return NotFound(new { error = ex.Message });
        //     }
        // }

        // POST: Area/CreateArea
        [HttpPost("CreateArea")]
        public async Task<IActionResult> AddArea(AreaNameDTO name)
        {
            try
            {
                if (name.AreaName.Length != 0)
                {
                    await _areaRepo.AddAreaAsync(name);
                    return Ok(new ObjectRespone(true, "add new success"));
                }
                return BadRequest(new ObjectRespone(false, "name cannot to blank" ));
            }
            catch (Exception ex) { return BadRequest(new { succes = false, message = ex.Message }); }
        }

        // PUT: Area/EditArea/{AreaCode}
        [HttpPut("EditArea/{areaCode}")]
        public async Task<IActionResult> EditArea(AreaNameDTO name, string areaCode)
        {
            try
            {
                await _areaRepo.EditAreaAsync(name, areaCode);
                return Ok(new ObjectRespone(true, "edit successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ObjectRespone(false, ex.Message));
            }
        }

        //[HttpDelete("DeleteArea/{id}")]
        //public async Task<IActionResult> DeleteArea(int id)
        //{
        //    await _areaRepo.DeleteAreaAsync(id);
        //    return Ok();
        //}
    }
}