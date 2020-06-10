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
    public class PostRepository : BaseRepository, IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TResult GetPost<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Category)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetPosts<TResult>(PostSearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IIncludableQueryable<Post, ICollection<PostVote>> query = _context.Posts
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.PostVotes);

            IQueryable<Post> whereParams = WhereConditionPredicate(query, searchParams);

            IOrderedQueryable<TResult> orderedQuery = searchParams.OrderByDir.ToUpper() == "DESC"
                ? whereParams
                    .ProjectTo<TResult>(configurationProvider)
                    .OrderByDescending(searchParams.SortBy)
                : whereParams
                    .ProjectTo<TResult>(configurationProvider)
                    .OrderBy(searchParams.SortBy);

            return orderedQuery
                .Skip(searchParams.Offset)
                .Take(searchParams.Limit)
                .ToList();
        }

        public IEnumerable<string> GetUserFollowings(string userId)
        {
            return _context.Followers
                .Where(x => x.FollowerUserId == userId)
                .Select(x => x.UserToFollowId)
                .ToList();
        }

        public int GetPostCount(PostSearchParams searchParams, IConfigurationProvider configurationProvider)
        {
            DbSet<Post> query = _context.Posts;
            IQueryable<Post> whereParams = WhereConditionPredicate(query, searchParams);

            return whereParams.Count();
        }

        public Task<int> CreatePost(Post post)
        {
            return CreateEntity(post);
        }

        public Task<int> UpdatePost(Post post)
        {
            return UpdateEntity(post);
        }

        public Task<int> DeletePost(Post post)
        {
            IIncludableQueryable<Post, IEnumerable<Comment>> comments =
                _context.Posts.Where(x => x.Id == post.Id).Include(x => x.Comments);
            _context.Posts.RemoveRange(comments);
            IIncludableQueryable<Post, IEnumerable<PostVote>> votes = _context.Posts.Where(x => x.Id == post.Id)
                .Include(x => x.PostVotes);
            _context.Posts.RemoveRange(votes);

            return DeleteEntity(post);
        }

        private static IQueryable<Post> WhereConditionPredicate(IQueryable<Post> query, PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                query = query.Where(x => x.Category.Name == searchParams.CategoryName);
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