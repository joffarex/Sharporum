using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public CategoryViewModel GetCategoryById(string categoryId)
        {
            var category = _categoryRepository.GetCategory<CategoryViewModel>(x => x.Id == categoryId,
                CategoryHelpers.GetCategoryMapperConfiguration());
            Guard.Against.NullItem(category, nameof(category));

            return category;
        }

        public CategoryViewModel GetCategoryByName(string categoryName)
        {
            var category = _categoryRepository.GetCategory<CategoryViewModel>(x => x.Name == categoryName,
                CategoryHelpers.GetCategoryMapperConfiguration());
            Guard.Against.NullItem(category, nameof(category));

            return category;
        }

        public Category GetCategoryEntity(string categoryId)
        {
            Category category = _categoryRepository.GetCategory(x => x.Id == categoryId);
            Guard.Against.NullItem(category, nameof(category));

            return category;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(CategorySearchParams searchParams)
        {
            return _categoryRepository.GetCategories<CategoryViewModel>(searchParams,
                CategoryHelpers.GetCategoryMapperConfiguration());
        }

        public async Task<int> GetCategoriesCountAsync(CategorySearchParams searchParams)
        {
            return _categoryRepository.GetCategoryCount(searchParams);
        }

        public async Task<string> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);

            await _categoryRepository.CreateCategoryAsync(category);

            return category.Id;
        }

        public async Task<CategoryViewModel> UpdateCategoryAsync(string categoryId,
            UpdateCategoryDto updateCategoryDto)
        {
            Category category = GetCategoryEntity(categoryId);
            category.Name = updateCategoryDto.Name;

            await _categoryRepository.UpdateCategoryAsync(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task DeleteCategoryAsync(string categoryId)
        {
            Category category = GetCategoryEntity(categoryId);
            await _categoryRepository.DeleteCategoryAsync(category);
        }
    }
}