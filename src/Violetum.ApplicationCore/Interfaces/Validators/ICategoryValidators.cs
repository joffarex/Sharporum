using System;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICategoryValidators
    {
        TResult GetCategoryByIdOrThrow<TResult>(string categoryId, Func<Category, TResult> selector);
        TResult GetCategoryByNameOrThrow<TResult>(string categoryName, Func<Category, TResult> selector);
    }
}