using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface ICategoryService
    {
        CategoryViewModel GetCategoryById(string categoryId);
        CategoryViewModel GetCategoryByName(string categoryName);
        Category GetCategoryEntity(string categoryId);
        Task<IEnumerable<CategoryViewModel>> GetCategories(CategorySearchParams searchParams);
        Task<int> GetTotalCategoriesCount(CategorySearchParams searchParams);

        Task<string> CreateCategory(string userId, CreateCategoryDto createCategoryDto);

        Task<CategoryViewModel> UpdateCategory(Category category, UpdateCategoryDto updateCategoryDto);

        Task<CategoryViewModel> UpdateCategoryImage(Category category, UpdateCategoryImageDto updateCategoryImageDto);

        Task DeleteCategory(Category category);
        Task AddModerator(Category category, AddModeratorDto addModeratorDto);
    }
}