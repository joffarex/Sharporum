using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryViewModel> GetCategoryById(string categoryId);
        Task<CategoryViewModel> GetCategoryByName(string categoryName);
        Task<Category> GetCategoryEntity(string categoryId);
        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(CategorySearchParams searchParams);
        Task<int> GetCategoriesCountAsync(CategorySearchParams searchParams);

        Task<string> CreateCategoryAsync(CreateCategoryDto createCategoryDto);

        Task<CategoryViewModel> UpdateCategoryAsync(string categoryId, UpdateCategoryDto updateCategoryDto);

        Task DeleteCategoryAsync(string categoryId);
    }
}