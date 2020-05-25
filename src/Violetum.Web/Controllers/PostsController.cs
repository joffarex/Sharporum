using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;
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
        public async Task<IActionResult> Details(string id, string commentSortBy, string commentDir, int commentPage)
        {
            ViewData["SortByParm"] = string.IsNullOrEmpty(commentSortBy) ? "CreatedAt" : commentSortBy;
            ViewData["OrderByDirParm"] = string.IsNullOrEmpty(commentDir) ? "desc" : commentDir;
            ViewData["CurrentPageParm"] = commentPage != 0 ? commentPage : 1;

            PostViewModel post = _postService.GetPost(id);

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            var searchParams = new CommentSearchParams
            {
                SortBy = (string) ViewData["SortByParm"],
                OrderByDir = (string) ViewData["OrderByDirParm"],
                CurrentPage = (int) ViewData["CurrentPageParm"],
                PostId = post.Id,
            };

            IEnumerable<CommentViewModel> comments = await _commentService.GetComments(searchParams);

            int totalComments = await _commentService.GetTotalCommentsCount(searchParams);
            ViewData["TotalComments"] = totalComments;
            var totalPages =
                (int) Math.Ceiling(totalComments / (double) searchParams.Limit);
            ViewData["totalPages"] = totalPages;

            var postPageViewModel = new PostPageViewModel
            {
                Post = post,
                Comments = comments,
            };

            return View(postPageViewModel);
        }

        public async Task<IActionResult> Index(string sortBy, string dir, int page, string title)
        {
            ViewData["SortByParm"] = string.IsNullOrEmpty(sortBy) ? "CreatedAt" : sortBy;
            ViewData["OrderByDirParm"] = string.IsNullOrEmpty(dir) ? "desc" : dir;
            ViewData["CurrentPageParm"] = page != 0 ? page : 1;
            ViewData["PostTitleParm"] = title;

            var searchParams = new PostSearchParams
            {
                SortBy = (string) ViewData["SortByParm"],
                OrderByDir = (string) ViewData["OrderByDirParm"],
                CurrentPage = (int) ViewData["CurrentPageParm"],
            };

            if (!string.IsNullOrEmpty(title))
            {
                searchParams.PostTitle = title;
            }

            IEnumerable<PostViewModel> posts = await _postService.GetPosts(searchParams);
            int totalPosts = await _postService.GetTotalPostsCount(searchParams);
            ViewData["TotalPosts"] = totalPosts;

            var totalPages =
                (int) Math.Ceiling(totalPosts / (double) searchParams.Limit);
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

            if (!string.IsNullOrEmpty(categoryId))
            {
                return View(new PostDto {CategoryId = categoryId});
            }

            return View(new PostDto());
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
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            try
            {
                await _postService.DeletePost(id, userId);

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
            [Bind("Direction")] PostVoteDto postVoteDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _postService.VotePost(postId, userId, postVoteDto);

            return RedirectToAction(nameof(Details), new {Id = postId});
        }
    }
}