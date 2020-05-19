using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
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

        public async Task<IActionResult> Details(string id, [Bind("CurrentPage,Limit")] Paginator paginator)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        public async Task<IActionResult> Index([Bind("UserId,CategoryName")] SearchParams searchParams,
            [Bind("CurrentPage,Limit")] Paginator paginator)
        {
            IEnumerable<PostViewModel> posts = await _postService.GetPosts(searchParams, paginator);

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            return View(posts);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;
            // TODO: populate model with categories
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content, CategoryId, AuthorId")]
            PostDto postDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PostViewModel post = await _postService.CreatePost(postDto);

                    return RedirectToAction(nameof(Details), new {post.Id});
                }

                return View(postDto);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [Authorize]
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                PostViewModel post = _postService.GetPost(id);
                return View(post);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Content")] UpdatePostDto updatePostDto)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                string userId = await _tokenManager.GetUserIdFromAccessToken();

                PostViewModel post = await _postService.UpdatePost(id, userId, updatePostDto);

                return RedirectToAction(nameof(Details), new {post.Id});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [Authorize]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                PostViewModel post = _postService.GetPost(id);
                return View(post);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, [Bind("Id")] DeletePostDto deletePostDto)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                string userId = await _tokenManager.GetUserIdFromAccessToken();

                await _postService.DeletePost(id, userId, deletePostDto);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
    }
}