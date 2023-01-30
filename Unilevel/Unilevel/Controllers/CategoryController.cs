using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Unilevel.Models;
using Unilevel.Services;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepositor;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepositor = categoryRepository;
        }

        [HttpGet("List")]
        public async Task<IActionResult> GetAllCategory()
        {
            var category = await _categoryRepositor.GetAllCategoryAsync();
            return Ok(category);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCategory(AddOrEditCategory category)
        {
            try
            {
                var categ = await _categoryRepositor.CreateCategoryAsync(category);
                return Ok(categ);
            }
            catch (Exception ex)
            {
                return BadRequest(new {Error = ex.Message});
            }
        }

        [HttpPut("Edit/{categoryId}")]
        public async Task<IActionResult> EditCategory(AddOrEditCategory category, int categoryId)
        {
            try
            {
                var categ = await _categoryRepositor.EditCategoryAsync(category, categoryId);
                return Ok(categ);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("Detail/{categoryId}")]
        public async Task<IActionResult> CategoryDetail(int categoryId)
        {
            try
            {
                var categ = await _categoryRepositor.CategoryDetailAsync(categoryId);
                return Ok(categ);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpDelete("Delete/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                await _categoryRepositor.DeleteCategoryAsync(categoryId);
                return Ok(new { Message = "Success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
