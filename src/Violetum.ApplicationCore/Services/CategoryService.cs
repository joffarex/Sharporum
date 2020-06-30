using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Category;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
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

        public async Task<CategoryViewModel> UpdateCategoryAsync(Category category,
            UpdateCategoryDto updateCategoryDto)
        {
            category.Name = updateCategoryDto.Name;

            await _categoryRepository.UpdateCategoryAsync(category);

            return _mapper.Map<CategoryViewModel>(category);
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            await _categoryRepository.DeleteCategoryAsync(category);
        }
    }
}