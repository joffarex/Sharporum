using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Domain.Infrastructure
{
    public interface IPostRepository
    {
        TResult GetPost<TResult>(Func<Post, bool> condition, Func<Post, TResult> selector);

        IEnumerable<TResult> GetPosts<TResult, TKey>(Func<Post, bool> condition, Func<Post, TResult> selector,
            Func<TResult, TKey> keySelector, PostSearchParams searchParams);

        int GetPostCount(Func<Post, bool> condition);

        Task<int> CreatePost(Post post);
        Task<int> UpdatePost(Post post);
        Task<int> DeletePost(Post post);

        int GetPostVoteSum(string postId);
    }
}