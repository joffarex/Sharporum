using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
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
            PostViewModel post = _postRepository.GetPostById(postId, x => _mapper.Map<PostViewModel>(x));
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            return post;
        }

        public async Task<IEnumerable<PostViewModel>> GetPosts(SearchParams searchParams, Paginator paginator)
        {
            await ValidateSearchParams(searchParams);

            return _postRepository
                .GetPosts(x => Predicate(searchParams.UserId, searchParams.CategoryName, x),
                    x => _mapper.Map<PostViewModel>(x), paginator);
        }

        public async Task<PostViewModel> CreatePost(PostDto postDto)
        {
            PostDtoValidationData validatedData = await ValidatePostDto(postDto);

            var post = _mapper.Map<Post>(postDto);
            post.Author = validatedData.User;
            post.Category = validatedData.Category;

            bool result = await _postRepository.CreatePost(post) > 0;

            if (!result)
            {
                throw new Exception("Create post failed");
            }

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task<PostViewModel> UpdatePost(string postId, string userId, UpdatePostDto updatePostDto)
        {
            Post post = ValidatePostActionData(postId, userId, updatePostDto.Id);

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;

            bool result = await _postRepository.UpdatePost(post) > 0;

            if (!result)
            {
                throw new Exception("update post failed");
            }

            return _mapper.Map<PostViewModel>(post);
        }

        public async Task<bool> DeletePost(string postId, string userId, DeletePostDto deletePostDto)
        {
            Post post = ValidatePostActionData(postId, userId, deletePostDto.Id);

            return await _postRepository.DeletePost(post) > 0;
        }


        private async Task ValidateSearchParams(SearchParams searchParams)
        {
            Category category = _categoryRepository.GetCategory(x => x.Name == searchParams.CategoryName);
            if (category == null)
            {
                throw new Exception("Category not found");
            }

            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            if (user == null)
            {
                throw new Exception("User not found");
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
                throw new Exception("User not found");
            }

            Category category = _categoryRepository.GetCategory(x => x.Id == postDto.CategoryId);
            if (category == null)
            {
                throw new Exception("Category not found");
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
                throw new Exception("Post data validation failed");
            }

            Post post = _postRepository.GetPostById(postId, x => x);
            if (post == null)
            {
                throw new Exception("Post not found");
            }

            if (post.AuthorId != userId)
            {
                throw new Exception("Unauthorized user");
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
}