using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface IPostService
    {
        PostViewModel GetPost(string postId);
        Task<IEnumerable<PostViewModel>> GetPosts(SearchParams searchParams, Paginator paginator);
        Task<PostViewModel> CreatePost(PostDto postDto);
        Task<PostViewModel> UpdatePost(string postId, string userId, UpdatePostDto updatePostDto);
        Task DeletePost(string postId, string userId, DeletePostDto deletePostDto);
        Task VotePost(string postId, string userId, PostVoteDto postVoteDto);
        int GetPostVoteSum(string postId);
    }
}