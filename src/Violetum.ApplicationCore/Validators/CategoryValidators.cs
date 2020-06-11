using System;
using System.Linq.Expressions;
using System.Net;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Validators
{
    [Validator]
    public class CategoryValidators : ICategoryValidators
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryValidators(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public TResult GetCategoryOrThrow<TResult>(Expression<Func<TResult, bool>> condition) where TResult : class
        {
            TResult category =
                _categoryRepository.GetCategory(condition, CategoryHelpers.GetCategoryMapperConfiguration());
            if (category == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Category)} not found");
            }

            return category;
        }
    }
}