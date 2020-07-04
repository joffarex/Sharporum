using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class PostService : IPostService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly UserManager<User> _userManager;
        private readonly IVoteRepository _voteRepository;

        public PostService(IPostRepository postRepository, IVoteRepository voteRepository,
            ICommunityRepository communityRepository, UserManager<User> userManager, IMapper mapper)
        {
            _postRepository = postRepository;
            _voteRepository = voteRepository;
            _communityRepository = communityRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public PostViewModel GetPost(string postId)
        {
            var post = _postRepository.GetPost<PostViewModel>(x => x.Id == postId,
                PostHelpers.GetPostMapperConfiguration());
            Guard.Against.NullItem(post, nameof(post));

            return post;
        }

        public Post GetPostEntity(string postId)
        {
            Post post = _postRepository.GetPost(x => x.Id == postId);
            Guard.Against.NullItem(post, nameof(post));

            return post;
        }

        public async Task<IEnumerable<PostViewModel>> GetPostsAsync(PostSearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user, nameof(user));

            Community community = _communityRepository.GetCommunity(x => x.Name == searchParams.CommunityName);
            Guard.Against.NullItem(community, nameof(community));

            return _postRepository.GetPosts<PostViewModel>(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public IEnumerable<PostViewModel> GetNewsFeedPosts(string userId, PostSearchParams searchParams)
        {
            Community community = _communityRepository.GetCommunity(x => x.Name == searchParams.CommunityName);
            Guard.Against.NullItem(community, nameof(community));

            searchParams.Followers = _postRepository.GetUserFollowings(userId);
            return _postRepository.GetPosts<PostViewModel>(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public async Task<int> GetPostsCountAsync(PostSearchParams searchParams)
        {
            Community community = _communityRepository.GetCommunity(x => x.Name == searchParams.CommunityName);
            Guard.Against.NullItem(community, nameof(community));

            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user.Id, nameof(user));

            return _postRepository.GetPostCount(searchParams);
        }

        public int GetPostsCountInNewsFeed(string userId, PostSearchParams searchParams)
        {
            Community community = _communityRepository.GetCommunity(x => x.Name == searchParams.CommunityName);
            Guard.Against.NullItem(community, nameof(community));

            searchParams.Followers = _postRepository.GetUserFollowings(userId);
            return _postRepository.GetPostCount(searchParams);
        }

        public async Task<string> CreatePostAsync(string userId, CreatePostDto createPostDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            var post = _mapper.Map<Post>(createPostDto);
            post.AuthorId = user.Id;
            post.ContentType = "application/text";

            await _postRepository.CreatePostAsync(post);

            return post.Id;
        }

        public async Task<Post> CreatePostWithFileAsync(string userId, CreatePostWithFileDto createPostWithFileDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            var post = _mapper.Map<Post>(createPostWithFileDto);
            post.AuthorId = user.Id;

            await _postRepository.CreatePostAsync(post);

            return post;
        }

        public async Task<PostViewModel> UpdatePostAsync(Post post, UpdatePostDto updatePostDto)
        {
            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;

            await _postRepository.UpdatePostAsync(post);

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task DeletePostAsync(Post post)
        {
            await _postRepository.DeletePostAsync(post);
        }

        public async Task VotePostAsync(string postId, string userId, PostVoteDto postVoteDto)
        {
            try
            {
                User user = await _userManager.FindByIdAsync(userId);
                Guard.Against.NullItem(user, nameof(user));

                Post post = _postRepository.GetPost(x => x.Id == postId);
                Guard.Against.NullItem(post, nameof(post));

                var postVote = _voteRepository.GetEntityVote<PostVote>(
                    x => (x.PostId == post.Id) && (x.UserId == user.Id),
                    x => x
                );

                if (postVote != null)
                {
                    postVote.Direction = postVote.Direction == postVoteDto.Direction ? 0 : postVoteDto.Direction;

                    await _voteRepository.UpdateEntityVoteAsync(postVote);
                }
                else
                {
                    var newPostVote = new PostVote
                    {
                        PostId = post.Id,
                        UserId = user.Id,
                        Direction = postVoteDto.Direction,
                    };

                    await _voteRepository.VoteEntityAsync(newPostVote);
                }
            }
            catch (ValidationException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity, e.Message);
            }
        }
    }
}