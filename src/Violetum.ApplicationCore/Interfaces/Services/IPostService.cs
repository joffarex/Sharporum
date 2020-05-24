using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IPostService
    {
        PostViewModel GetPost(string postId);
        Task<IEnumerable<PostViewModel>> GetPosts(PostSearchParams searchParams);
        Task<int> GetTotalPostsCount(PostSearchParams searchParams);

        Task<PostViewModel> CreatePost(PostDto postDto);
        Task<PostViewModel> UpdatePost(string postId, string userId, UpdatePostDto updatePostDto);
        Task DeletePost(string postId, string userId);
        Task VotePost(string postId, string userId, PostVoteDto postVoteDto);
        int GetPostVoteSum(string postId);
    }
}