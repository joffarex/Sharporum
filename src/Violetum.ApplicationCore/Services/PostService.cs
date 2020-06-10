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

        public async Task<IEnumerable<PostViewModel>> GetPosts(PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrow(searchParams.UserId);
            }

            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                _categoryValidators.GetCategoryByNameOrThrow(searchParams.CategoryName, x => x);
            }

            return _postRepository.GetPosts<PostViewModel>(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public IEnumerable<PostViewModel> GetNewsFeedPosts(string userId, PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                _categoryValidators.GetCategoryByNameOrThrow(searchParams.CategoryName, x => x);
            }

            searchParams.Followers = _postRepository.GetUserFollowings(userId);
            return _postRepository.GetPosts<PostViewModel>(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public async Task<int> GetTotalPostsCount(PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                _categoryValidators.GetCategoryByNameOrThrow(searchParams.CategoryName, x => x);
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrow(searchParams.UserId);
            }

            return _postRepository.GetPostCount(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public int GetTotalPostsCountInNewsFeed(string userId, PostSearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                _categoryValidators.GetCategoryByNameOrThrow(searchParams.CategoryName, x => x);
            }

            searchParams.Followers = _postRepository.GetUserFollowings(userId);
            return _postRepository.GetPostCount(searchParams, PostHelpers.GetPostMapperConfiguration());
        }

        public async Task<PostViewModel> CreatePost(string userId, CreatePostDto createPostDto)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);
            Category category = _categoryValidators.GetCategoryByIdOrThrow(createPostDto.CategoryId, x => x);

            var post = _mapper.Map<Post>(createPostDto);
            post.Author = user;
            post.Category = category;

            await _postRepository.CreatePost(post);

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task<PostViewModel> UpdatePost(PostViewModel postViewModel, UpdatePostDto updatePostDto)
        {
            var post = _postValidators.GetPostOrThrow<Post>(x => x.Id == postViewModel.Id);
            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;

            await _postRepository.UpdatePost(post);

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task DeletePost(PostViewModel postViewModel)
        {
            await _postRepository.DeletePost(_mapper.Map<Post>(postViewModel));
        }

        public async Task VotePost(string postId, string userId, PostVoteDto postVoteDto)
        {
            try
            {
                User user = await _userValidators.GetUserByIdOrThrow(userId);
                var post = _postValidators.GetPostOrThrow<Post>(x => x.Id == postId);

                var postVote =
                    _voteRepository.GetEntityVote<PostVote>(
                        x => (x.PostId == post.Id) && (x.UserId == user.Id), x => x);

                if (postVote != null)
                {
                    if (postVote.Direction == postVoteDto.Direction)
                    {
                        postVote.Direction = 0;
                    }
                    else
                    {
                        postVote.Direction = postVoteDto.Direction;
                    }

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

                    newPostVote.User = user;
                    newPostVote.Post = post;

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