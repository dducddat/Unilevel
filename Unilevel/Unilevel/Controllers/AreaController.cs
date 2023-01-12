using Microsoft.AspNetCore.Mvc;
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

        // GET: Area/List
        [HttpGet("List")]
         public async Task<IActionResult> GetAllArea()
        {
            return Ok(await _areaRepo.GetAllAreaAsync());
        }

        // GET: Area/Detail/{areaCode}
        [HttpGet("Detail/{areaCode}")]
         public async Task<IActionResult> GetAreaById(string areaCode)
        {
            try
            {
                var area = await _areaRepo.GetAreaAsync(areaCode);
                return Ok(area);
            }
            catch (Exception ex)
            {
                return NotFound(new { Error = ex.Message });
            }
        }

        // POST: Area/Create
        [HttpPost("Create")]
        public async Task<IActionResult> AddArea(AddOrEditArea area)
        {
            try
            {
                if (area.AreaName != string.Empty && area.AreaName != null)
                {
                    await _areaRepo.AddAreaAsync(area);
                    return Ok(new { Message = "Successfully added new" });
                }
                else
                {
                    return BadRequest(new { Error = "name cannot to blank" });
                }
            }
            catch (Exception ex) { return Problem(detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError); }
        }

        // PUT: Area/Edit/{areaCode}
        [HttpPut("Edit/{areaCode}")]
        public async Task<IActionResult> EditArea(AddOrEditArea area, string areaCode)
        {
            try
            {
                if (area.AreaName == string.Empty && area.AreaName == null) 
                { 
                    return BadRequest(new { Error = "name cannot be blank" }); 
                }
                await _areaRepo.EditAreaAsync(area, areaCode);
                return Ok(new {Message = "Edit successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // DELETE: Area/Delete/{areaCode}
        [HttpDelete("Delete/{areaCode}")]
        public async Task<IActionResult> DeleteArea(string areaCode)
        {
            try
            {
                await _areaRepo.DeleteAreaAsync(areaCode);
                return Ok(new { Message = "Successful delete" });
            }
            catch(Exception ex) 
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}