using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class PostRepository : BaseRepository, IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TResult GetPostById<TResult>(string postId, Func<Post, TResult> selector)
        {
            return _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Category)
                .Where(x => x.Id == postId)
                .Select(selector)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetPosts<TResult>(Expression<Func<Post, bool>> condition,
            Func<Post, TResult> selector, Paginator paginator)
        {
            return _context.Posts.Include(x => x.Category)
                .Where(condition)
                .Select(selector)
                .Skip(paginator.Offset)
                .Take(paginator.Limit)
                .ToList();
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
            return DeleteEntity(post);
        }
    }
}