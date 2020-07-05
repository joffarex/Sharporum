using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface IPostService
    {
        Task<PostViewModel> GetPost(string postId);
        Task<Post> GetPostEntity(string postId);
        Task<IEnumerable<PostViewModel>> GetPostsAsync(PostSearchParams searchParams);
        Task<IEnumerable<PostViewModel>> GetNewsFeedPosts(string userId, PostSearchParams searchParams);
        Task<int> GetPostsCountAsync(PostSearchParams searchParams);
        Task<int> GetPostsCountInNewsFeed(string userId, PostSearchParams searchParams);

        Task<string> CreatePostAsync(string userId, CreatePostDto createPostDto);
        Task<Post> CreatePostWithFileAsync(string userId, CreatePostWithFileDto createPostWithFileDto);
        Task<PostViewModel> UpdatePostAsync(Post post, UpdatePostDto updatePostDto);
        Task DeletePostAsync(Post post);
        Task VotePostAsync(string postId, string userId, PostVoteDto postVoteDto);
    }
}