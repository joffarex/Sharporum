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
        private readonly ICategoryValidators _categoryValidators;
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly IPostValidators _postValidators;
        private readonly IUserValidators _userValidators;
        private readonly IVoteRepository _voteRepository;

        public PostService(IPostRepository postRepository, IVoteRepository voteRepository, IMapper mapper,
            IPostValidators postValidators, ICategoryValidators categoryValidators, IUserValidators userValidators)
        {
            _postRepository = postRepository;
            _voteRepository = voteRepository;
            _mapper = mapper;
            _postValidators = postValidators;
            _categoryValidators = categoryValidators;
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

        public async Task<IEnumerable<PostViewModel>> GetPosts(PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrow(searchParams.UserId);
            }

            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                _categoryValidators.GetCategoryOrThrow(x => x.Name == searchParams.CategoryName);
            }

            return _postRepository.GetPosts<PostViewModel>(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public IEnumerable<PostViewModel> GetNewsFeedPosts(string userId, PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                _categoryValidators.GetCategoryOrThrow(x => x.Name == searchParams.CategoryName);
            }

            searchParams.Followers = _postRepository.GetUserFollowings(userId);
            return _postRepository.GetPosts<PostViewModel>(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public async Task<int> GetTotalPostsCount(PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                _categoryValidators.GetCategoryOrThrow(x => x.Name == searchParams.CategoryName);
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrow(searchParams.UserId);
            }

            return _postRepository.GetPostCount(searchParams);
        }

        public int GetTotalPostsCountInNewsFeed(string userId, PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                _categoryValidators.GetCategoryOrThrow(x => x.Name == searchParams.CategoryName);
            }

            searchParams.Followers = _postRepository.GetUserFollowings(userId);
            return _postRepository.GetPostCount(searchParams);
        }

        public async Task<string> CreatePost(string userId, CreatePostDto createPostDto)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);

            var post = _mapper.Map<Post>(createPostDto);
            post.AuthorId = user.Id;

            await _postRepository.CreatePost(post);

            return post.Id;
        }

        public async Task<PostViewModel> UpdatePost(Post post, UpdatePostDto updatePostDto)
        {
            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;

            await _postRepository.UpdatePost(post);

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task DeletePost(Post post)
        {
            await _postRepository.DeletePost(post);
        }

        public async Task VotePost(string postId, string userId, PostVoteDto postVoteDto)
        {
            try
            {
                User user = await _userValidators.GetUserByIdOrThrow(userId);
                Post post = _postValidators.GetPostOrThrow(x => x.Id == postId);

                var postVote =
                    _voteRepository.GetEntityVote<PostVote>(
                        x => (x.PostId == post.Id) && (x.UserId == user.Id), x => x);

                if (postVote != null)
                {
                    postVote.Direction = postVote.Direction == postVoteDto.Direction ? 0 : postVoteDto.Direction;

                    await _voteRepository.UpdateEntityVote(postVote);
                }
                else
                {
                    var newPostVote = new PostVote
                    {
                        PostId = post.Id,
                        UserId = user.Id,
                        Direction = postVoteDto.Direction,
                    };

                    await _voteRepository.VoteEntity(newPostVote);
                }
            }
            catch (ValidationException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity, e.Message);
            }
        }
    }
}