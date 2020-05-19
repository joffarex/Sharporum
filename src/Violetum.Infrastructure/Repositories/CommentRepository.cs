using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public TResult GetCommentById<TResult>(string commentId, Func<Comment, TResult> selector)
        {
            return _context.Comments
                .Include(x => x.Author)
                .Include(x => x.Post)
                .Where(x => x.Id == commentId)
                .Select(selector)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetComments<TResult>(Func<Comment, bool> condition,
            Func<Comment, TResult> selector, Paginator paginator)
        {
            return _context.Comments
                .Include(x => x.Author)
                .AsEnumerable()
                .Where(condition)
                .Select(selector)
                .Skip(paginator.Offset)
                .Take(paginator.Limit)
                .ToList();
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