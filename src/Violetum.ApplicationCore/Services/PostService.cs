using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class PostService : IPostService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        private readonly IPostRepository _postRepository;
        private readonly UserManager<User> _userManager;

        public PostService(IPostRepository postRepository, UserManager<User> userManager,
            ICategoryRepository categoryRepository, IMapper mapper)
        {
            _postRepository = postRepository;
            _userManager = userManager;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public PostViewModel GetPost(string postId)
        {
            PostViewModel post = _postRepository.GetPostById(postId, x => AttachVotesToPostViewModel(x));
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Post)}:{postId} not found");
            }

            return post;
        }

        public async Task<IEnumerable<PostViewModel>> GetPosts(SearchParams searchParams, Paginator paginator)
        {
            await ValidateSearchParams(searchParams);

            return _postRepository
                .GetPosts(x => Predicate(searchParams.UserId, searchParams.CategoryName, x),
                    x => AttachVotesToPostViewModel(x), paginator);
        }

        public async Task<PostViewModel> CreatePost(PostDto postDto)
        {
            PostDtoValidationData validatedData = await ValidatePostDto(postDto);

            var post = _mapper.Map<Post>(postDto);
            post.Author = validatedData.User;
            post.Category = validatedData.Category;

            await _postRepository.CreatePost(post);

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task<PostViewModel> UpdatePost(string postId, string userId, UpdatePostDto updatePostDto)
        {
            Post post = ValidatePostActionData(postId, userId, updatePostDto.Id);

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;

            await _postRepository.UpdatePost(post);

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task DeletePost(string postId, string userId, DeletePostDto deletePostDto)
        {
            Post post = ValidatePostActionData(postId, userId, deletePostDto.Id);

            await _postRepository.DeletePost(post);
        }

        public async Task VotePost(string postId, string userId, PostVoteDto postVoteDto)
        {
            try
            {
                PostVoteDtoValidationData validatedData = await ValidatePostVoteActionData(postId, userId, postVoteDto);

                PostVote postVote = _postRepository.GetPostVote(postId, userId, x => x);

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

                    await _postRepository.UpdatePostVote(postVote);
                }
                else
                {
                    var newPostVote = new PostVote
                    {
                        PostId = postVoteDto.PostId,
                        UserId = postVoteDto.UserId,
                        Direction = postVoteDto.Direction,
                    };

                    newPostVote.User = validatedData.User;
                    newPostVote.Post = validatedData.Post;

                    await _postRepository.VotePost(newPostVote);
                }
            }
            catch (ValidationException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity, e.Message);
            }
        }

        public int GetPostVoteSum(string postId)
        {
            Post post = _postRepository.GetPostById(postId, x => x);
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Post)}:{postId} not found");
            }

            return _postRepository.GetPostVoteSum(post.Id);
        }

        private PostViewModel AttachVotesToPostViewModel(Post x)
        {
            var postViewModel = _mapper.Map<PostViewModel>(x);
            postViewModel.VoteCount = GetPostVoteSum(x.Id);
            return postViewModel;
        }

        private async Task<PostVoteDtoValidationData> ValidatePostVoteActionData(string postId, string userId,
            PostVoteDto postVoteDto)
        {
            if (postVoteDto == null)
            {
                throw new ArgumentNullException(nameof(postVoteDto));
            }

            User user = await _userManager.FindByIdAsync(postVoteDto.UserId);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(User)}:{postVoteDto.UserId} not found");
            }

            Post post = _postRepository.GetPostById(postVoteDto.PostId, x => x);
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Post)}:{postVoteDto.PostId} not found");
            }

            return new PostVoteDtoValidationData
            {
                User = user,
                Post = post,
            };
        }

        private async Task ValidateSearchParams(SearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                Category category = _categoryRepository.GetCategory(x => x.Name == searchParams.CategoryName);
                if (category == null)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                        $"{nameof(Category)}:{searchParams.CategoryName} not found");
                }
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                User user = await _userManager.FindByIdAsync(searchParams.UserId);
                if (user == null)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                        $"{nameof(User)}:{searchParams.UserId} not found");
                }
            }
        }

        private async Task<PostDtoValidationData> ValidatePostDto(PostDto postDto)
        {
            if (postDto == null)
            {
                throw new ArgumentNullException(nameof(postDto));
            }

            User user = await _userManager.FindByIdAsync(postDto.AuthorId);
            if (user == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(User)}:{postDto.AuthorId} not found");
            }

            Category category = _categoryRepository.GetCategory(x => x.Id == postDto.CategoryId);
            if (category == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                    $"{nameof(Category)}:{postDto.CategoryId} not found");
            }

            return new PostDtoValidationData
            {
                User = user,
                Category = category,
            };
        }

        private Post ValidatePostActionData(string postId, string userId, string dtoPostId)
        {
            if (postId != dtoPostId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity,
                    $"{MethodBase.GetCurrentMethod().Name} failed");
            }

            Post post = _postRepository.GetPostById(postId, x => x);
            if (post == null)
            {
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, $"{nameof(Post)}:{postId} not found");
            }

            if (post.AuthorId != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            return post;
        }

        private static bool Predicate(string userId, string categoryName, Post p)
        {
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(categoryName))
            {
                return (p.Category.Name == categoryName) && (p.AuthorId == userId);
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                return p.Category.Name == categoryName;
            }

            if (!string.IsNullOrEmpty(userId))
            {
                return p.AuthorId == userId;
            }

            return true;
        }
    }

    public class PostDtoValidationData
    {
        public User User { get; set; }
        public Category Category { get; set; }
    }

    public class PostVoteDtoValidationData
    {
        public User User { get; set; }
        public Post Post { get; set; }
    }
}