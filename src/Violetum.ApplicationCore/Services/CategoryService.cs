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
            return _categoryValidators.GetCategoryByIdOrThrow(categoryId,
                x => _mapper.Map<CategoryViewModel>(x));
        }

        public CategoryViewModel GetCategoryByName(string categoryName)
        {
            return _categoryValidators.GetCategoryByNameOrThrow(categoryName,
                x => _mapper.Map<CategoryViewModel>(x));
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategories(CategorySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrow(searchParams.UserId);
            }

            return _categoryRepository.GetCategories(
                x => CategoryHelpers.WhereConditionPredicate(searchParams, x),
                x => _mapper.Map<CategoryViewModel>(x), searchParams);
        }

        public async Task<int> GetTotalCategoriesCount(CategorySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrow(searchParams.UserId);
            }

            return _categoryRepository.GetTotalCommentsCount(
                x => CategoryHelpers.WhereConditionPredicate(searchParams, x)
            );
        }

        public async Task<CategoryViewModel> CreateCategory(string userId, CreateCategoryDto createCategoryDto)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);

            var category = _mapper.Map<Category>(createCategoryDto);
            category.Author = user;

            await _categoryRepository.CreateCategory(category);

            await CreateCategoryAdminRole(user, category.Id);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<CategoryViewModel> UpdateCategory(CategoryViewModel categoryViewModel,
            UpdateCategoryDto updateCategoryDto)
        {
            var category = _mapper.Map<Category>(categoryViewModel);

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;

            await _categoryRepository.UpdateCategory(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<CategoryViewModel> UpdateCategoryImage(CategoryViewModel categoryViewModel,
            UpdateCategoryImageDto updateCategoryImageDto)
        {
            var category = _mapper.Map<Category>(categoryViewModel);

            await _categoryRepository.UpdateCategory(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<bool> DeleteCategory(CategoryViewModel categoryViewModel)
        {
            await RemoveCategoryRoles(categoryViewModel.Id);

            Category category = _categoryValidators.GetCategoryByIdOrThrow(categoryViewModel.Id, x => x);
            return await _categoryRepository.DeleteCategory(category) > 0;
        }

        public async Task AddModerator(CategoryViewModel categoryViewModel, AddModeratorDto addModeratorDto)
        {
            string roleName = $"{nameof(Category)}/{categoryViewModel.Id}/{Roles.Moderator}";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest);
            }

            User newModerator = await _userManager.FindByIdAsync(addModeratorDto.NewModeratorId);
            await _userManager.AddToRoleAsync(newModerator, roleName);
        }

        private async Task CreateCategoryAdminRole(User user, string categoryId)
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

        private async Task RemoveCategoryRoles(string categoryId)
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