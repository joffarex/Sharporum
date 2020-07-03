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
        CategoryViewModel GetCategoryById(string categoryId);
        CategoryViewModel GetCategoryByName(string categoryName);
        Category GetCategoryEntity(string categoryId);
        Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(CategorySearchParams searchParams);
        Task<int> GetCategoriesCountAsync(CategorySearchParams searchParams);

        Task<string> CreateCategoryAsync(CreateCategoryDto createCategoryDto);

        Task<CategoryViewModel> UpdateCategoryAsync(string categoryId, UpdateCategoryDto updateCategoryDto);

        Task DeleteCategoryAsync(string categoryId);
    }
}