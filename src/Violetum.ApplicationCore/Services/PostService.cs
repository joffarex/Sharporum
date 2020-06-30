using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
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
        private readonly ICommunityValidators _communityValidators;
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IPostValidators _postValidators;
        private readonly IUserValidators _userValidators;
        private readonly IVoteRepository _voteRepository;

        public PostService(IPostRepository postRepository, IVoteRepository voteRepository, IMapper mapper,
            IPostValidators postValidators, ICommunityValidators communityValidators, IUserValidators userValidators)
        {
            _postRepository = postRepository;
            _voteRepository = voteRepository;
            _mapper = mapper;
            _postValidators = postValidators;
            _communityValidators = communityValidators;
            _userValidators = userValidators;
        }

        public PostViewModel GetPost(string postId)
        {
            return _postValidators.GetPostOrThrow<PostViewModel>(x => x.Id == postId);
        }

        public Post GetPostEntity(string postId)
        {
            return _postValidators.GetPostOrThrow(x => x.Id == postId);
        }

        public async Task<IEnumerable<PostViewModel>> GetPostsAsync(PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrowAsync(searchParams.UserId);
            }

            if (!string.IsNullOrEmpty(searchParams.CommunityName))
            {
                _communityValidators.GetCommunityOrThrow(x => x.Name == searchParams.CommunityName);
            }

            return _postRepository.GetPosts<PostViewModel>(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public IEnumerable<PostViewModel> GetNewsFeedPosts(string userId, PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CommunityName))
            {
                _communityValidators.GetCommunityOrThrow(x => x.Name == searchParams.CommunityName);
            }

            searchParams.Followers = _postRepository.GetUserFollowings(userId);
            return _postRepository.GetPosts<PostViewModel>(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public async Task<int> GetPostsCountAsync(PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CommunityName))
            {
                _communityValidators.GetCommunityOrThrow(x => x.Name == searchParams.CommunityName);
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrowAsync(searchParams.UserId);
            }

            return _postRepository.GetPostCount(searchParams);
        }

        public int GetPostsCountInNewsFeed(string userId, PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CommunityName))
            {
                _communityValidators.GetCommunityOrThrow(x => x.Name == searchParams.CommunityName);
            }

            searchParams.Followers = _postRepository.GetUserFollowings(userId);
            return _postRepository.GetPostCount(searchParams);
        }

        public async Task<string> CreatePostAsync(string userId, CreatePostDto createPostDto)
        {
            User user = await _userValidators.GetUserByIdOrThrowAsync(userId);

            var post = _mapper.Map<Post>(createPostDto);
            post.AuthorId = user.Id;

            await _postRepository.CreatePostAsync(post);

            return post.Id;
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
                User user = await _userValidators.GetUserByIdOrThrowAsync(userId);
                Post post = _postValidators.GetPostOrThrow(x => x.Id == postId);

                var postVote =
                    _voteRepository.GetEntityVote<PostVote>(
                        x => (x.PostId == post.Id) && (x.UserId == user.Id), x => x);

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