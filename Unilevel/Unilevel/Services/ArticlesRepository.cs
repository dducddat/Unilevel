using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class ArticlesRepository : IArticlesRepository
    {
        private readonly UnilevelContext _context;

        public ArticlesRepository(UnilevelContext context)
        {
            _context = context;
        }

        public async Task<ArticlesDetail> AddArticlesAsync(ArticlesBrief articlesBrief, string userId)
        {
            Articles articles = new Articles();
            articles.Title = articlesBrief.Title;
            articles.Description = articlesBrief.Description;
            articles.Hypertext = articlesBrief.Hypertext;
            articles.Status = false;
            articles.CreateDate = DateTime.Now;
            articles.CreateByUserId = userId;
            articles.Thumbnail = articlesBrief.Thumbnail;

            _context.Add(articles);
            await _context.SaveChangesAsync();

            var user = await _context.Users.Where(u => u.Id == userId)
                                                   .Select(u => new { FullName = u.FullName })
                                                   .FirstOrDefaultAsync();

            return new ArticlesDetail {
                Id = articles.Id,
                Title = articles.Title,
                Description = articles.Description,
                Hypertext = articles.Hypertext,
                CreateByUser = user.FullName,
                CreateDate = articles.CreateDate.ToString("dd/MM/yyyy"),
                Thumbnail = articles.Thumbnail,
                Status = articles.Status ? "Published" : "Disabled"
            };
        }

        public async Task<string> DeleteArticlesAsync(int articlesId)
        {
            var articles = await _context.Articles.SingleOrDefaultAsync(a => a.Id == articlesId);

            if (articles is null)
                return string.Empty;

            _context.Remove(articles);
            await _context.SaveChangesAsync();

            return "Successful";
        }

        public async Task<ArticlesDetail?> EditArticlesAsync(int articlesId, ArticlesDetail articlesDetail)
        {
            var articles = await _context.Articles.Include(a => a.CreateByUser).SingleOrDefaultAsync(a => a.Id == articlesId);

            if (articles == null)
                return null;

            articles.Title = articlesDetail.Title;
            articles.Hypertext = articlesDetail.Hypertext;
            articles.Description = articlesDetail.Description;
            articles.Thumbnail = articlesDetail.Thumbnail;

            _context.Update(articles);
            await _context.SaveChangesAsync();

            return new ArticlesDetail
            {
                Id = articles.Id,
                Title = articles.Title,
                Description = articles.Description,
                Hypertext = articles.Hypertext,
                CreateByUser = articles.CreateByUser.FullName,
                CreateDate = articles.CreateDate.ToString("dd/MM/yyyy"),
                Thumbnail = articles.Thumbnail,
                Status = articles.Status ? "Published" : "Disabled"
            };
        }

        public async Task<List<ArticlesModel>> GetAllArticlesAsync()
        {
            var lstArticles = await _context.Articles.Include(a => a.CreateByUser).ToListAsync();

            List<ArticlesModel> result = new List<ArticlesModel>();

            if (lstArticles != null)
            {
                foreach (var articles in lstArticles)
                {
                    result.Add(new ArticlesModel { 
                        Id = articles.Id,  
                        CreateByUser = articles.CreateByUser.FullName,
                        CreateDate = articles.CreateDate.ToString("dd/MM/yyyy"),
                        Title = articles.Title,
                        Status = articles.Status ? "Published" : "Disabled"
                    });
                }
            }

            return result;
        }

        public async Task<ArticlesDetail?> GetArticlesByIdAsync(int articlesId)
        {
            var articles = await _context.Articles.Include(a => a.CreateByUser).SingleOrDefaultAsync(a => a.Id == articlesId);

            if (articles is null)
                return null;

            return new ArticlesDetail
            {
                Id = articles.Id,
                Title = articles.Title,
                Description = articles.Description,
                Hypertext = articles.Hypertext,
                CreateByUser = articles.CreateByUser.FullName,
                CreateDate = articles.CreateDate.ToString("dd/MM/yyyy"),
                Thumbnail = articles.Thumbnail,
                Status = articles.Status ? "Published" : "Disabled"
            };
        }

        public async Task<string> UploadArticlesAsync(int articlesId)
        {
            var articles = await _context.Articles.SingleOrDefaultAsync(a => a.Id == articlesId);

            if (articles == null)
                return string.Empty;

            articles.Status = true;
            _context.Update(articles);
            await _context.SaveChangesAsync();

            return "Successful";
        }
    }
}
