using System.Collections.Generic;
using System.Threading.Tasks;
using Sharporum.Core.Dtos.Category;
using Sharporum.Core.ViewModels.Category;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryViewModel> GetCategoryByIdAsync(string categoryId);
        Task<CategoryViewModel> GetCategoryByNameAsync(string categoryName);
        Task<Category> GetCategoryEntityAsync(string categoryId);
        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(CategorySearchParams searchParams);
        Task<int> GetCategoriesCountAsync(CategorySearchParams searchParams);

        Task<string> CreateCategoryAsync(CreateCategoryDto createCategoryDto);

        Task<CategoryViewModel> UpdateCategoryAsync(string categoryId, UpdateCategoryDto updateCategoryDto);

        Task DeleteCategoryAsync(string categoryId);
    }
}