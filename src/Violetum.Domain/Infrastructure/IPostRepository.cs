using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface IPostRepository
    {
        TResult GetPostById<TResult>(string postId, Func<Post, TResult> selector);

        IEnumerable<TResult> GetPosts<TResult>(Expression<Func<Post, bool>> condition, Func<Post, TResult> selector,
            Paginator paginator);

        Task<int> CreatePost(Post post);
        Task<int> UpdatePost(Post post);
        Task<int> DeletePost(Post post);
    }
}