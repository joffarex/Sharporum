using System;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface IPostValidators
    {
        TResult GetPostByIdOrThrow<TResult>(string postId, Func<Post, TResult> selector);
    }
}