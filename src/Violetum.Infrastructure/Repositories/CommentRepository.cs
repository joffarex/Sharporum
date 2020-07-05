using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;
using Violetum.Infrastructure.Extensions;

namespace Violetum.Infrastructure.Repositories
{
    [Repository]
    public class CommentRepository : IAsyncRepository<Comment>
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Comments
                .Include(x => x.Author)
                .Include(x => x.Post)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<Comment> GetByConditionAsync(Expression<Func<Comment, bool>> condition)
        {
            return await _context.Comments
                .Include(x => x.Post)
                .Include(x => x.Author)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISearchParams<Comment> searchParams,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IIncludableQueryable<Comment, ICollection<CommentVote>> query = _context.Comments
                .Include(x => x.Author)
                .Include(x => x.CommentVotes);

            // IQueryable<Comment> whereParams = WhereConditionPredicate(query, searchParams);

            IOrderedQueryable<TResult> orderedQuery = searchParams.OrderByDir.ToUpper() == "DESC"
                ? query
                    .ProjectTo<TResult>(configurationProvider)
                    .OrderByDescending(searchParams.SortBy)
                : query
                    .ProjectTo<TResult>(configurationProvider)
                    .OrderBy(searchParams.SortBy);

            return await orderedQuery
                .Skip(searchParams.Offset)
                .Take(searchParams.Limit)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(ISearchParams<Comment> searchParams)
        {
            DbSet<Comment> query = _context.Comments;
            // IQueryable<Comment> whereParams = WhereConditionPredicate(query, searchParams);

            return await query.CountAsync();
        }

        public async Task CreateAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            IIncludableQueryable<Comment, IEnumerable<CommentVote>> votes =
                _context.Comments.Where(x => x.Id == comment.Id).Include(x => x.CommentVotes);
            _context.Comments.RemoveRange(votes);

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();
        }

        private static IQueryable<Comment> WhereConditionPredicate(IQueryable<Comment> query,
            CommentSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                query = query.Where(x => x.AuthorId == searchParams.UserId);
            }

            if (!string.IsNullOrEmpty(searchParams.PostId))
            {
                query = query.Where(x => x.PostId == searchParams.PostId);
            }

            return query;
        }
    }
}