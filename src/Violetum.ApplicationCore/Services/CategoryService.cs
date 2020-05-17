﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
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

        public CategoryViewModel GetCategoryByName(string categoryName)
        {
            CategoryViewModel category =
                _categoryRepository.GetCategory(x => x.Name == categoryName, x => _mapper.Map<CategoryViewModel>(x));
            if (category == null)
            {
                throw new Exception("Category not found");
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

            bool result = await _categoryRepository.CreateCategory(category) > 0;
            if (!result)
            {
                throw new Exception("Create category fialed");
            }

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<CategoryViewModel> UpdateCategory(string categoryName, string userId,
            UpdateCategoryDto updateCategoryDto)
        {
            Category category = ValidateCategoryActionData(categoryName, userId, updateCategoryDto.Id);

            category.Name = updateCategoryDto.Name;
            category.Description = updateCategoryDto.Description;
            category.Image = updateCategoryDto.Image;

            bool result = await _categoryRepository.UpdateCategory(category) > 0;

            if (!result)
            {
                throw new Exception("update post failed");
            }

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task<bool> DeleteCategory(string categoryName, string userId, DeleteCategoryDto deleteCategoryDto)
        {
            Category category = ValidateCategoryActionData(categoryName, userId, deleteCategoryDto.Id);

            return await _categoryRepository.DeleteCategory(category) > 0;
        }

        private async Task ValidateSearchParams(SearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
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
                throw new ArgumentNullException(nameof(categoryDto));
            }

            User user = await _userManager.FindByIdAsync(categoryDto.AuthorId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            return new CategoryDtoValidationDatas
            {
                User = user,
            };
        }

        private Category ValidateCategoryActionData(string categoryName, string userId,
            string dtoCategoryId)
        {
            Category category =
                _categoryRepository.GetCategory(x => (x.Name == categoryName) && (x.Id == dtoCategoryId), x => x);
            if (category == null)
            {
                throw new Exception("Post not found");
            }

            if (category.AuthorId != userId)
            {
                throw new Exception("Unauthorized user");
            }

            return category;
        }
    }

    public class CategoryDtoValidationDatas
    {
        public User User { get; set; }
    }
}