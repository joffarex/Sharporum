using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.Domain.Entities;

namespace Violetum.Domain.Infrastructure
{
    public interface IPostRepository
    {
        TResult GetPostById<TResult>(string postId, Func<Post, TResult> selector);

        IEnumerable<TResult> GetPosts<TResult>(Func<Post, bool> condition, Func<Post, TResult> selector,
            Paginator paginator);

        Task<int> CreatePost(Post post);
        Task<int> UpdatePost(Post post);
        Task<int> DeletePost(Post post);

        TResult GetPostVote<TResult>(string postId, string userId, Func<PostVote, TResult> selector);
        int GetPostVoteSum(string postId);
        Task<int> VotePost(PostVote postVote);
        Task<int> UpdatePostVote(PostVote postVote);
    }
}