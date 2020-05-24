using System;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces.Validators
{
    public interface ICommentValidators
    {
        TResult GetReturnedCommentOrThrow<TResult>(string commentId, Func<Comment, TResult> selector);
    }
}