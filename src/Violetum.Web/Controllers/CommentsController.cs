using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Infrastructure;

namespace Violetum.Web.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly ITokenManager _tokenManager;

        public CommentsController(ICommentService commentService, ITokenManager tokenManager)
        {
            _commentService = commentService;
            _tokenManager = tokenManager;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content, PostId, AuthorId")] CommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Posts", new {Id = commentDto.PostId});
            }

            try
            {
                CommentViewModel comment = await _commentService.CreateComment(commentDto);

                return RedirectToAction("Details", "Posts", new {comment.Post.Id});
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
            CommentViewModel comment = _commentService.GetComment(id);
            if (comment.Author.Id != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized);
            }

            return View(comment);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Content")] UpdateCommentDto updateCommentDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            try
            {
                CommentViewModel comment = await _commentService.UpdateComment(id, userId, updateCommentDto);

                return RedirectToAction("Details", "Posts", new {comment.Post.Id});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id, string postId,
            [Bind("Id")] DeleteCommentDto deleteCommentDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            try
            {
                await _commentService.DeleteComment(id, userId, deleteCommentDto);

                return RedirectToAction("Details", "Posts", new {id = postId});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}