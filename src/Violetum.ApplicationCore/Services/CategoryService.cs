using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public CategoryService(ICategoryRepository categoryRepository, UserManager<User> userManager, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public CategoryViewModel GetCategoryById(string id)
        {
            CategoryViewModel category =
                _categoryRepository.GetCategory(x => x.Id == id, x => _mapper.Map<CategoryViewModel>(x));
            if (category == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Category)} not found");
            }

            return category;
        }

        public CategoryViewModel GetCategoryByName(string categoryName)
        {
            CategoryViewModel category =
                _categoryRepository.GetCategory(x => x.Name == categoryName, x => _mapper.Map<CategoryViewModel>(x));
            if (category == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Category)} not found");
            }

            return category;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategories(SearchParams searchParams, Paginator paginator)
        {
            await ValidateSearchParams(searchParams);

            return _categoryRepository.GetCategories(x => Predicate(searchParams.UserId, searchParams.CategoryName, x),
                x => _mapper.Map<CategoryViewModel>(x), paginator);
        }

        public async Task<CategoryViewModel> CreateCategory(CategoryDto categoryDto)
        {
            CategoryDtoValidationDatas validatedData = await ValidateCategoryDto(categoryDto);

            var category = _mapper.Map<Category>(categoryDto);
            category.Author = validatedData.User;

            await _categoryRepository.CreateCategory(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<CategoryViewModel> UpdateCategory(string id, string userId,
            UpdateCategoryDto updateCategoryDto)
        {
            Category category = ValidateCategoryActionData(id, userId, updateCategoryDto.Id);

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;
            category.Image = updateCategoryDto.Image;

            await _categoryRepository.UpdateCategory(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<bool> DeleteCategory(string id, string userId, DeleteCategoryDto deleteCategoryDto)
        {
            Category category = ValidateCategoryActionData(id, userId, deleteCategoryDto.Id);

            return await _categoryRepository.DeleteCategory(category) > 0;
        }

        private async Task ValidateSearchParams(SearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                User user = await _userManager.FindByIdAsync(searchParams.UserId);
                if (user == null)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(User)} not found");
                }
            }
        }

        private static bool Predicate(string userId, string categoryName, Category c)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(categoryName))
            {
                return c.Name.Contains(categoryName) && (c.AuthorId == userId);
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                return c.Name.Contains(categoryName);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                return c.AuthorId == userId;
            }

            return true;
        }

        private async Task<CategoryDtoValidationDatas> ValidateCategoryDto(CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest);
            }

            User user = await _userManager.FindByIdAsync(categoryDto.AuthorId);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(User)} not found");
            }

            return new CategoryDtoValidationDatas
            {
                User = user,
            };
        }

        private Category ValidateCategoryActionData(string id, string userId,
            string dtoCategoryId)
        {
            Category category =
                _categoryRepository.GetCategory(x => (x.Id == id) && (x.Id == dtoCategoryId), x => x);
            if (category == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Category)} not found");
            }

            if (category.AuthorId != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);
            }

            return category;
        }
    }

    public class CategoryDtoValidationDatas
    {
        public User User { get; set; }
    }
}