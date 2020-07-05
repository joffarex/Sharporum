using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Specifications;
using Violetum.ApplicationCore.ViewModels.Category;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IAsyncRepository<Category> _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IAsyncRepository<Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryViewModel> GetCategoryByIdAsync(string categoryId)
        {
            var category = await _categoryRepository.GetByConditionAsync<CategoryViewModel>(x => x.Id == categoryId,
                CategoryHelpers.GetCategoryMapperConfiguration());
            Guard.Against.NullItem(category, nameof(category));

            return category;
        }

        public async Task<CategoryViewModel> GetCategoryByNameAsync(string categoryName)
        {
            var category = await _categoryRepository.GetByConditionAsync<CategoryViewModel>(x => x.Name == categoryName,
                CategoryHelpers.GetCategoryMapperConfiguration());
            Guard.Against.NullItem(category, nameof(category));

            return category;
        }

        public async Task<Category> GetCategoryEntityAsync(string categoryId)
        {
            Category category = await _categoryRepository.GetByConditionAsync(x => x.Id == categoryId);
            Guard.Against.NullItem(category, nameof(category));

            return category;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetCategoriesAsync(CategorySearchParams searchParams)
        {
            var specification = new CategoryFilterSpecification(searchParams);

            return await _categoryRepository.ListAsync<CategoryViewModel>(specification,
                CategoryHelpers.GetCategoryMapperConfiguration());
        }

        public async Task<int> GetCategoriesCountAsync(CategorySearchParams searchParams)
        {
            var specification = new CategoryFilterSpecification(searchParams);

            return await _categoryRepository.GetTotalCountAsync(specification);
        }

        public async Task<string> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = _mapper.Map<Category>(createCategoryDto);

            await _categoryRepository.CreateAsync(category);

            return category.Id;
        }

        public async Task<CategoryViewModel> UpdateCategoryAsync(string categoryId,
            UpdateCategoryDto updateCategoryDto)
        {
            Category category = await GetCategoryEntityAsync(categoryId);
            category.Name = updateCategoryDto.Name;

            await _categoryRepository.UpdateAsync(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task DeleteCategoryAsync(string categoryId)
        {
            Category category = await GetCategoryEntityAsync(categoryId);
            await _categoryRepository.DeleteAsync(category);
        }
    }
}