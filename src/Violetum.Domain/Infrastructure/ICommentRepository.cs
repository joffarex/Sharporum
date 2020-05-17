using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface ICommentRepository
    {
        IEnumerable<TResult> GetComments<TResult>(Expression<Func<Comment, bool>> predicate,
            Func<Comment, TResult> selector,
            Paginator paginator);

        Task<int> CreateComment(Comment comment);
        Task<int> UpdateComment(Comment comment);
        Task<int> DeleteComment(Comment comment);
    }
}