using System;
using System.Net;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Validators
{
    public class CategoryValidators : ICategoryValidators
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryValidators(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public TResult GetCategoryByIdOrThrow<TResult>(string categoryId, Func<Category, TResult> selector)
        {
            TResult post = _categoryRepository.GetCategory(x => x.Id == categoryId, selector);
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Category)}:{categoryId} not found");
            }

            return post;
        }

        public TResult GetCategoryByNameOrThrow<TResult>(string categoryName, Func<Category, TResult> selector)
        {
            TResult post = _categoryRepository.GetCategory(x => x.Name == categoryName, selector);
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Category)}:{categoryName} not found");
            }

            return post;
        }
    }
}