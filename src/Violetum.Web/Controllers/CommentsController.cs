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
            ViewData["PostId"] = postId;
            // TODO: populate model with categories

            if (!string.IsNullOrEmpty(parentId))
            {
                return View(new CreateCommentDto {ParentId = parentId});
            }

            return View(new CreateCommentDto());
        }

        [Authorize]
        [HttpPost("Comment/{postId}/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string postId,
            [Bind("Content, ParentId, AuthorId, PostId")]
            CreateCommentDto createCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Create));
            }

            try
            {
                createCommentDto.PostId = postId;
                CommentViewModel comment = await _commentService.CreateComment(createCommentDto);

                TempData["CreateCommentSuccess"] = "Comment successfully created";
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

            return View(new UpdateCommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.Post.Id,
            });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,PostId,Content")] UpdateCommentDto updateCommentDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit));
            }

            string userId = await _tokenManager.GetUserIdFromAccessToken();
            try
            {
                CommentViewModel comment = await _commentService.UpdateComment(id, userId, updateCommentDto);

                TempData["EditCommentSuccess"] = "Comment successfully edited";
                ;
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

                TempData["DeleteCommentSuccess"] = "Comment successfully deleted";
                ;
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
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Posts", new {Id = commentVoteDto.PostId});
            }

            string userId = await _tokenManager.GetUserIdFromAccessToken();

            await _commentService.VoteComment(commentId, userId, commentVoteDto);

            return RedirectToAction("Details", "Posts", new {Id = commentVoteDto.PostId});
        }
    }
}