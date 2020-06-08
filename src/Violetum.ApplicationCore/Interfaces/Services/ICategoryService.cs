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

        Task<CategoryViewModel> CreateCategory(string userId, CreateCategoryDto createCategoryDto);

        Task<CategoryViewModel> UpdateCategory(CategoryViewModel categoryViewModel,
            UpdateCategoryDto updateCategoryDto);

        Task<CategoryViewModel> UpdateCategoryImage(CategoryViewModel categoryViewModel,
            UpdateCategoryImageDto updateCategoryImageDto);

        Task<bool> DeleteCategory(CategoryViewModel categoryViewModel);
        Task AddModerator(CategoryViewModel categoryViewModel, AddModeratorDto addModeratorDto);
    }
}