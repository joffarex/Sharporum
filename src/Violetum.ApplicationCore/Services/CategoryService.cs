﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryValidators _categoryValidators;
        private readonly IMapper _mapper;
        private readonly IUserValidators _userValidators;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper,
            ICategoryValidators categoryValidators, IUserValidators userValidators)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
            _categoryValidators = categoryValidators;
            _userValidators = userValidators;
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

        public async Task<CategoryViewModel> CreateCategory(CreateCategoryDto createCategoryDto)
        {
            if (string.IsNullOrEmpty(createCategoryDto.AuthorId))
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, "Unauthorized User");
            }

            User user = await _userValidators.GetUserByIdOrThrow(createCategoryDto.AuthorId);

            var category = _mapper.Map<Category>(createCategoryDto);
            category.Author = user;

            await _categoryRepository.CreateCategory(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<CategoryViewModel> UpdateCategory(string categoryId, string userId,
            UpdateCategoryDto updateCategoryDto)
        {
            if (categoryId != updateCategoryDto.Id)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                    $"{nameof(Comment)}:(catid[{categoryId}]|dtoid[{updateCategoryDto.Id}] update");
            }

            Category category = _categoryValidators.GetCategoryByIdOrThrow(categoryId, x => x);

            bool userOwnsCategory = CategoryHelpers.UserOwnsCategory(userId, category.AuthorId);
            if (!userOwnsCategory)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized,
                    $"Unauthorized User:{userId ?? "Anonymous"}");
            }

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;
            category.Image = updateCategoryDto.Image;

            await _categoryRepository.UpdateCategory(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<bool> DeleteCategory(string categoryId, string userId)
        {
            Category category = _categoryValidators.GetCategoryByIdOrThrow(categoryId, x => x);

            bool userOwnsCategory = CategoryHelpers.UserOwnsCategory(userId, category.AuthorId);
            if (!userOwnsCategory)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized,
                    $"Unauthorized User:{userId ?? "Anonymous"}");
            }

            return await _categoryRepository.DeleteCategory(category) > 0;
        }
    }
}