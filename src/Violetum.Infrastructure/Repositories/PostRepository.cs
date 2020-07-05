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
    public class PostRepository : IAsyncRepository<Post>
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Community)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<Post> GetByConditionAsync(Expression<Func<Post, bool>> condition)
        {
            return await _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Community)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISearchParams<Post> searchParams,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IIncludableQueryable<Post, ICollection<PostVote>> query = _context.Posts
                .Include(x => x.Community)
                .Include(x => x.Author)
                .Include(x => x.PostVotes);

            // IQueryable<Post> whereParams = WhereConditionPredicate(query, searchParams);

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

        public async Task<int> GetTotalCountAsync(ISearchParams<Post> searchParams)
        {
            DbSet<Post> query = _context.Posts;
            // IQueryable<Post> whereParams = WhereConditionPredicate(query, searchParams);

            return await query.CountAsync();
        }

        public async Task CreateAsync(Post post)
        {
            await _context.Posts.AddAsync(post);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            IQueryable<Comment> comments = _context.Comments.Where(x => x.PostId == post.Id);
            _context.Comments.RemoveRange(comments);

            IIncludableQueryable<Post, IEnumerable<PostVote>> votes = _context.Posts.Where(x => x.Id == post.Id)
                .Include(x => x.PostVotes);
            _context.Posts.RemoveRange(votes);

            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();
        }

        private static IQueryable<Post> WhereConditionPredicate(IQueryable<Post> query, PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CommunityName))
            {
                query = query.Where(x => x.Community.Name == searchParams.CommunityName);
            }

            if (!string.IsNullOrEmpty(searchParams.PostTitle))
            {
                query = query.Where(x => x.Title.Contains(searchParams.PostTitle));
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                query = query.Where(x => x.AuthorId == searchParams.UserId);
            }

            if (searchParams.Followers != null)
            {
                query = query.Where(x => searchParams.Followers.Contains(x.AuthorId));
            }

            return query;
        }
    }
}