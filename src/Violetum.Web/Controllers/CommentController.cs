using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Infrastructure;

namespace Violetum.Web.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ITokenManager _tokenManager;

        public CommentController(ICommentService commentService, ITokenManager tokenManager)
        {
            _commentService = commentService;
            _tokenManager = tokenManager;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content, PostId, AuthorId")] CommentDto commentDto)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CommentViewModel comment = await _commentService.CreateComment(commentDto);

                    return RedirectToAction("Details", "Post", new {Id = commentDto.PostId});
                }

                return BadRequest();
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
                CommentViewModel comment = _commentService.GetComment(id);
                return View(comment);
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,Content")] UpdateCommentDto updateCommentDto)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                string userId = await _tokenManager.GetUserIdFromAccessToken();

                CommentViewModel comment = await _commentService.UpdateComment(id, userId, updateCommentDto);

                return RedirectToAction("Details", "Post", new {comment.Post.Id});
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
        public async Task<IActionResult> Delete(string id, string postId,
            [Bind("Id")] DeleteCommentDto deleteCommentDto)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                string userId = await _tokenManager.GetUserIdFromAccessToken();

                await _commentService.DeleteComment(id, userId, deleteCommentDto);

                return RedirectToAction("Details", "Post", new {id = postId});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }
    }
}