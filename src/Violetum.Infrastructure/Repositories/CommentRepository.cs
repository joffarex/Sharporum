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
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<TResult> GetComments<TResult>(Expression<Func<Comment, bool>> predicate,
            Func<Comment, TResult> selector, Paginator paginator)
        {
            return GetEntities(predicate, selector, paginator);
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