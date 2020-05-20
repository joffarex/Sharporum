using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface ICategoryService
    {
        CategoryViewModel GetCategoryById(string id);
        CategoryViewModel GetCategoryByName(string categoryName);
        Task<IEnumerable<CategoryViewModel>> GetCategories(SearchParams searchParams, Paginator paginator);
        Task<CategoryViewModel> CreateCategory(CategoryDto categoryDto);
        Task<CategoryViewModel> UpdateCategory(string id, string userId, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategory(string id, string userId, DeleteCategoryDto deleteCategoryDto);
    }
}