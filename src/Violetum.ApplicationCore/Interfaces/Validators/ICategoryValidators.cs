using System;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICategoryValidators
    {
        TResult GetReturnedCategoryByIdOrThrow<TResult>(string categoryId, Func<Category, TResult> selector);
        TResult GetReturnedCategoryByNameOrThrow<TResult>(string categoryName, Func<Category, TResult> selector);
    }
}