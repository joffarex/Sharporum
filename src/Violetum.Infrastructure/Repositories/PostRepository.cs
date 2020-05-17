using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
            return _context.Posts.Where(x => x.Id == postId).Select(selector).FirstOrDefault();
        }

        public IEnumerable<TResult> GetPosts<TResult>(Expression<Func<Post, bool>> predicate,
            Func<Post, TResult> selector, Paginator paginator)
        {
            return GetEntities(predicate, selector, paginator);
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