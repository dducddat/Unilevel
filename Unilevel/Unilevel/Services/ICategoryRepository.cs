using Unilevel.Models;

namespace Unilevel.Services
{
    public interface ICategoryRepository
    {
        public Task<List<CategoryDetail>> GetAllCategoryAsync();
        public Task<CategoryDetail> CategoryDetailAsync(int categoryId);
        public Task<CategoryDetail> CreateCategoryAsync(AddOrEditCategory category);
        public Task<CategoryDetail> EditCategoryAsync(AddOrEditCategory category, int categoryId);
        public Task DeleteCategoryAsync(int categoryId);
    }
}
