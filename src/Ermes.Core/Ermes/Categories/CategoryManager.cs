using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ermes.Categories
{
    public class CategoryManager : DomainService
    {
        public IQueryable<Category> Categories { get { return CategoryRepository.GetAllIncluding(c => c.Translations); } }
        protected IRepository<Category> CategoryRepository { get; set; }
        public IQueryable<CategoryTranslation> CategoriesTranslation { get { return CategoryTranslationRepository.GetAllIncluding(ct => ct.Core); } }
        protected IRepository<CategoryTranslation> CategoryTranslationRepository { get; set; }
        public CategoryManager(IRepository<Category> categoryRepository, IRepository<CategoryTranslation> categoryTranslationRepository)
        {
            CategoryRepository = categoryRepository;
            CategoryTranslationRepository = categoryTranslationRepository;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await Categories.SingleOrDefaultAsync(a => a.Id == categoryId);
        }

        public async Task<Category> GetCategoryByCodeAsync(string categoryCode)
        {
            return await Categories.SingleOrDefaultAsync(a => a.Code == categoryCode);
        }
        public async Task<CategoryTranslation> GetCategoryTranslationByCoreIdLanguageAsync(int coreId, string language)
        {
            return await CategoriesTranslation.SingleOrDefaultAsync(a => a.CoreId == coreId && a.Language == language);
        }
        public async Task InsertOrUpdateCategoryAsync(Category category)
        {
            await CategoryRepository.InsertOrUpdateAsync(category);
        }
        public async Task InsertOrUpdateCategoryTranslationAsync(CategoryTranslation categoryTranslation)
        {
            await CategoryTranslationRepository.InsertOrUpdateAsync(categoryTranslation);
        }
    }
}
