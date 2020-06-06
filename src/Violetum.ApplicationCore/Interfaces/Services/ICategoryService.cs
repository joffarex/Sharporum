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
        Task<int> GetTotalCategoriesCount(CategorySearchParams searchParams);

        Task<CategoryViewModel> CreateCategory(CreateCategoryDto createCategoryDto);
        Task<CategoryViewModel> UpdateCategory(string id, string userId, UpdateCategoryDto updateCategoryDto);

        Task<CategoryViewModel> UpdateCategory(CategoryViewModel categoryViewModel,
            UpdateCategoryDto updateCategoryDto);

        Task<bool> DeleteCategory(string id, string userId);
        Task<bool> DeleteCategory(CategoryViewModel categoryViewModel);
    }
}