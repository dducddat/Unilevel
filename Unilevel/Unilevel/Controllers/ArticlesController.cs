using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unilevel.Models;
using Unilevel.Services;
using System.Security.Claims;

namespace Unilevel.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticlesRepository _articles;

        public ArticlesController(IArticlesRepository articles)
        {
            _articles = articles;
        }

        // GET: Articels/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllArticles()
        {
            var result = await _articles.GetAllArticlesAsync();

            return Ok(result);
        }

        // GET: Articels/GetArticles/{articlesId}
        [HttpGet("GetArticles/{articlesId}")]
        public async Task<IActionResult> GetArticlesById(int articlesId)
        {
            var result = await _articles.GetArticlesByIdAsync(articlesId);

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        // POST: Articles/Create
        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> AddArticles(ArticlesBrief articlesBrief)
        {
            var userId = User.FindFirstValue("id");

            if (userId == null || userId == string.Empty)
            {
                return Unauthorized();
            }

            if (articlesBrief.Title == string.Empty ||
               articlesBrief.Hypertext == string.Empty ||
               articlesBrief.Description == string.Empty ||
               articlesBrief.Thumbnail == string.Empty)
            {
                return BadRequest(new { Error = "Ivalid input: cannot be left blank" });
            }

            var result = await _articles.AddArticlesAsync(articlesBrief, userId);

            return Ok(new { Message = "Success", Data = result });
        }

        // DELETE: Articles/Delete
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteArticles(int articlesId)
        {
            var result = await _articles.DeleteArticlesAsync(articlesId);
            
            if(result == string.Empty)
                return NotFound();

            return Ok(result);
        }

        // PUT: Articles/Upload/{articlesId}
        [HttpPut("Upload/{articlesId}")]
        public async Task<IActionResult> UploadArticles(int articlesId)
        {
            var result = await _articles.UploadArticlesAsync(articlesId);

            if (result == string.Empty)
                return NotFound();

            return Ok(result);
        }

        // PUT: Articles/Edit/{articlesId}
        [HttpPut("Edit/{articlesId}")]
        public async Task<IActionResult> EditArticles(int articlesId, ArticlesDetail articlesDetail)
        {
            if (articlesDetail.Title == "" ||
                articlesDetail.Hypertext == "" ||
                articlesDetail.Description == "" ||
                articlesDetail.Thumbnail == "")
                return BadRequest("Ivalid input: cannot be left blank");

            var result = await _articles.EditArticlesAsync(articlesId, articlesDetail);

            if (articlesDetail.Id != articlesId)
                return BadRequest("Invalid");

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}
