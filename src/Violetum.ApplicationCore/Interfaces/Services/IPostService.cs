using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IPostService
    {
        PostViewModel GetPost(string postId);
        Post GetPostEntity(string postId);
        Task<IEnumerable<PostViewModel>> GetPosts(PostSearchParams searchParams);
        IEnumerable<PostViewModel> GetNewsFeedPosts(string userId, PostSearchParams searchParams);
        Task<int> GetTotalPostsCount(PostSearchParams searchParams);
        int GetTotalPostsCountInNewsFeed(string userId, PostSearchParams searchParams);

        Task<string> CreatePost(string userId, CreatePostDto createPostDto);
        Task<PostViewModel> UpdatePost(Post post, UpdatePostDto updatePostDto);
        Task DeletePost(Post post);
        Task VotePost(string postId, string userId, PostVoteDto postVoteDto);
    }
}