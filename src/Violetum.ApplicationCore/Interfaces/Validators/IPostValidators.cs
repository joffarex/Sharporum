using System;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface IPostValidators
    {
        TResult GetReturnedPostOrThrow<TResult>(string postId, Func<Post, TResult> selector);
    }
}