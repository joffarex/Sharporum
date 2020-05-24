using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface ICategoryService
    {
        CategoryViewModel GetCategoryById(string id);
        CategoryViewModel GetCategoryByName(string categoryName);
        Task<IEnumerable<CategoryViewModel>> GetCategories(CategorySearchParams searchParams);
        Task<CategoryViewModel> CreateCategory(CategoryDto categoryDto);
        Task<CategoryViewModel> UpdateCategory(string id, string userId, UpdateCategoryDto updateCategoryDto);
        Task<bool> DeleteCategory(string id, string userId);
    }
}