using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryValidators _categoryValidators;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserValidators _userValidators;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper,
            ICategoryValidators categoryValidators, IUserValidators userValidators,
            RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _categoryValidators = categoryValidators;
            _userValidators = userValidators;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public CategoryViewModel GetCategoryById(string categoryId)
        {
            return _categoryValidators.GetCategoryOrThrow<CategoryViewModel>(x => x.Id == categoryId);
        }

        public CategoryViewModel GetCategoryByName(string categoryName)
        {
            return _categoryValidators.GetCategoryOrThrow<CategoryViewModel>(x => x.Name == categoryName);
        }

        public Category GetCategoryEntity(string categoryId)
        {
            return _categoryValidators.GetCategoryOrThrow(x => x.Id == categoryId);
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(CategorySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrowAsync(searchParams.UserId);
            }

            return _categoryRepository.GetCategories<CategoryViewModel>(searchParams,
                CategoryHelpers.GetCategoryMapperConfiguration());
        }

        public async Task<int> GetCategoriesCountAsync(CategorySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrowAsync(searchParams.UserId);
            }

            return _categoryRepository.GetCategoryCount(searchParams);
        }

        public async Task<string> CreateCategoryAsync(string userId, CreateCategoryDto createCategoryDto)
        {
            User user = await _userValidators.GetUserByIdOrThrowAsync(userId);

            var category = _mapper.Map<Category>(createCategoryDto);
            category.Author = user;

            await _categoryRepository.CreateCategoryAsync(category);

            await CreateCategoryAdminRoleAsync(user, category.Id);

            return category.Id;
        }

        public async Task<CategoryViewModel> UpdateCategoryAsync(Category category,
            UpdateCategoryDto updateCategoryDto)
        {
            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;

            await _categoryRepository.UpdateCategoryAsync(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<CategoryViewModel> UpdateCategoryImageAsync(Category category,
            UpdateCategoryImageDto updateCategoryImageDto)
        {
            category.Image = updateCategoryImageDto.Image;

            await _categoryRepository.UpdateCategoryAsync(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            await RemoveCategoryRolesAsync(category.Id);

            await _categoryRepository.DeleteCategoryAsync(category);
        }

        public async Task AddModeratorAsync(Category category, AddModeratorDto addModeratorDto)
        {
            string roleName = $"{nameof(Category)}/{category.Id}/{Roles.Moderator}";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest);
            }

            User newModerator = await _userManager.FindByIdAsync(addModeratorDto.NewModeratorId);
            await _userManager.AddToRoleAsync(newModerator, roleName);
        }

        private async Task CreateCategoryAdminRoleAsync(User user, string categoryId)
        {
            string roleName = $"{nameof(Category)}/{categoryId}/{Roles.Admin}";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            else
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest);
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        private async Task RemoveCategoryRolesAsync(string categoryId)
        {
            string roleBase = $"{nameof(Category)}/{categoryId}";

            var roles = new List<string>
            {
                $"{roleBase}/{Roles.Admin}",
                $"{roleBase}/{Roles.Moderator}",
            };

            foreach (string roleName in roles)
            {
                IList<User> roleUsers = await _userManager.GetUsersInRoleAsync(roleName);
                foreach (User user in roleUsers)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }

                IdentityRole role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                {
                    continue;
                }

                IdentityResult identityResult = await _roleManager.DeleteAsync(role);

                if (!identityResult.Succeeded)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                        $"Something went wrong while removing category:{categoryId} roles");
                }
            }
        }
    }
}