using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IArticlesRepository
    {
        Task<List<ArticlesModel>> GetAllArticlesAsync();

        Task<ArticlesDetail> AddArticlesAsync(ArticlesBrief articlesBrief, string userId);

        Task<ArticlesDetail?> GetArticlesByIdAsync(int articlesId);

        Task<ArticlesDetail?> EditArticlesAsync(int articlesId, ArticlesDetail articlesDetail);

        Task<string> DeleteArticlesAsync(int articlesId);

        Task<string> UploadArticlesAsync(int articlesId);
    }
}
