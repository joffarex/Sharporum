using System.Collections.Generic;
using System.Threading.Tasks;
using Sharporum.Core.Dtos.Post;
using Sharporum.Core.ViewModels.Post;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Interfaces
{
    public interface IPostService
    {
        Task<PostViewModel> GetPostByIdAsync(string postId);
        Task<Post> GetPostEntityAsync(string postId);
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