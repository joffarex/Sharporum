using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<TResult> GetComments<TResult>(Expression<Func<Comment, bool>> condition,
            Func<Comment, TResult> selector, Paginator paginator)
        {
            return GetEntities(condition, selector, paginator);
        }

        public Task<int> CreateComment(Comment comment)
        {
            return CreateEntity(comment);
        }

        public Task<int> UpdateComment(Comment comment)
        {
            return UpdateEntity(comment);
        }

        public Task<int> DeleteComment(Comment comment)
        {
            return DeleteEntity(comment);
        }
    }
}