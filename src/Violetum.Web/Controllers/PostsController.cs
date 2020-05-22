using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Web.Models;

namespace Violetum.Web.Controllers
{
    public class PostsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IPostService _postService;
        private readonly ITokenManager _tokenManager;

        public PostsController(IPostService postService, ICommentService commentService, ITokenManager tokenManager)
        {
            _postService = postService;
            _commentService = commentService;
            _tokenManager = tokenManager;
        }

        [HttpGet("Posts/{id}")]
        public async Task<IActionResult> Details(string id, [Bind("CurrentPage,Limit")] Paginator paginator)
        {
            PostViewModel post = _postService.GetPost(id);

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            IEnumerable<CommentViewModel> comments = await _commentService.GetComments(new SearchParams
            {
                PostId = post.Id,
            }, paginator);

            var postPageViewModel = new PostPageViewModel
            {
                Post = post,
                Comments = comments,
            };

            return View(postPageViewModel);
        }

        public async Task<IActionResult> Index(string sortBy, string dir, int page)
        {
            ViewData["SortByparm"] = string.IsNullOrEmpty(sortBy) ? "CreatedAt" : sortBy;
            ViewData["OrderByDirParm"] = string.IsNullOrEmpty(dir) ? "desc" : dir;
            ViewData["CurrentPageParm"] = page != 0 ? page : 1;

            var searchParams = new SearchParams
            {
                SortBy = (string) ViewData["SortByparm"],
                OrderByDir = (string) ViewData["OrderByDirParm"],
                CurrentPage = (int) ViewData["CurrentPageParm"],
            };

            IEnumerable<PostViewModel> posts = await _postService.GetPosts(searchParams);
            var totalPages =
                (int) Math.Ceiling(await _postService.GetTotalPostsCount(searchParams) / (double) searchParams.Limit);
            ViewData["totalPages"] = totalPages;

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            return View(posts);
        }

        [Authorize]
        public async Task<IActionResult> Create([FromQuery] string categoryId)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;
            // TODO: populate model with categories

            if (string.IsNullOrEmpty(categoryId))
            {
                return View(new PostDto {CategoryId = categoryId});
            }

            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content, CategoryId, AuthorId")]
            PostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                return View(postDto);
            }

            try
            {
                PostViewModel post = await _postService.CreatePost(postDto);

                return RedirectToAction(nameof(Details), new {post.Id});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            PostViewModel post = _postService.GetPost(id);
            if (post.Author.Id != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);
            }

            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Content")] UpdatePostDto updatePostDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            try
            {
                PostViewModel post = await _postService.UpdatePost(id, userId, updatePostDto);

                return RedirectToAction(nameof(Details), new {post.Id});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            PostViewModel post = _postService.GetPost(id);
            if (post.Author.Id != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);
            }

            return View(post);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, [Bind("Id")] DeletePostDto deletePostDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            try
            {
                await _postService.DeletePost(id, userId, deletePostDto);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VotePost(string postId,
            [Bind("PostId,UserId,Direction")] PostVoteDto postVoteDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _postService.VotePost(postId, userId, postVoteDto);

            return RedirectToAction(nameof(Details), new {Id = postVoteDto.PostId});
        }
    }
}