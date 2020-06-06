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
        IEnumerable<PostViewModel> GetNewsFeedPosts(string userId, PostSearchParams searchParams);
        Task<int> GetTotalPostsCount(PostSearchParams searchParams);
        int GetTotalPostsCountInNewsFeed(string userId, PostSearchParams searchParams);

        Task<PostViewModel> CreatePost(CreatePostDto createPostDto);
        Task<PostViewModel> UpdatePost(string postId, string userId, UpdatePostDto updatePostDto);
        Task<PostViewModel> UpdatePost(PostViewModel postViewModel, UpdatePostDto updatePostDto);
        Task DeletePost(string postId, string userId);
        Task DeletePost(PostViewModel postViewModel);
        Task VotePost(string postId, string userId, PostVoteDto postVoteDto);
        int GetPostVoteSum(string postId);
    }
}