using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly UnilevelContext _Context;
        private readonly IMapper _mapper;

        public CategoryRepository(UnilevelContext Context, IMapper mapper) 
        {
            _Context = Context;
            _mapper = mapper;
        }

        public async Task<CategoryDetail> CategoryDetailAsync(int categoryId)
        {
            var category = await _Context.Categories
                .Where(c => c.Id == categoryId)
                .Where(c => c.Remove == false)
                .SingleOrDefaultAsync();

            if (category == null)
                throw new Exception("Category not found");

            return _mapper.Map<CategoryDetail>(category);
        }

        public async Task<CategoryDetail> CreateCategoryAsync(AddOrEditCategory category)
        {
            if (category.Name == string.Empty)
                throw new Exception("Name cannot be blank");

            Category categ = new Category { Name = category.Name, Remove = false };

            _Context.Add(categ);
            await _Context.SaveChangesAsync();

            return _mapper.Map<CategoryDetail>(categ);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            var categ = await _Context.Categories.SingleOrDefaultAsync(c => c.Id == categoryId);

            if (categ == null)
                throw new Exception("Category not found");

            categ.Remove = true;

            _Context.Update(categ);
            await _Context.SaveChangesAsync();
        }

        public async Task<CategoryDetail> EditCategoryAsync(AddOrEditCategory category, int categoryId)
        {
            var categ = await _Context.Categories.SingleOrDefaultAsync(c => c.Id == categoryId);

            if (categ == null)
                throw new Exception("Category not found");

            if (category.Name == string.Empty)
                throw new Exception("Name cannot be blank");

            categ.Name = category.Name;

            _Context.Update(categ);
            await _Context.SaveChangesAsync();

            return new CategoryDetail { Id = categ.Id, Name = categ.Name };
        }

        public async Task<List<CategoryDetail>> GetAllCategoryAsync()
        {
            var categorys = await _Context.Categories.Where(c => c.Remove == false).ToListAsync();

            return _mapper.Map<List<CategoryDetail>>(categorys);
        }
    }
}
