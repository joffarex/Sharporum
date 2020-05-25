using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Comment;
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
        [HttpGet("Comment/{postId}/Create")]
        public async Task<IActionResult> Create(string postId, [FromQuery] string parentId)
        {
            if (string.IsNullOrEmpty(postId))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest);
            }

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;
            // TODO: populate model with categories

            if (!string.IsNullOrEmpty(parentId))
            {
                var commentDto = new CommentDto {ParentId = parentId};
                return View(commentDto);
            }

            return View();
        }

        [Authorize]
        [HttpPost("Comment/{postId}/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string postId,
            [Bind("Content, ParentId, AuthorId")] CommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Posts", new {Id = commentDto.PostId});
            }

            try
            {
                commentDto.PostId = postId;
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
        public async Task<IActionResult> Delete(string id, string postId)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            try
            {
                await _commentService.DeleteComment(id, userId);

                return RedirectToAction("Details", "Posts", new {id = postId});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VoteComment(string commentId,
            [Bind("PostId,Direction")] CommentVoteDto commentVoteDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _commentService.VoteComment(commentId, userId, commentVoteDto);

            return RedirectToAction("Details", "Posts", new {Id = commentVoteDto.PostId});
        }
    }
}