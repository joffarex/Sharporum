using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Sharporum.API.Authorization;
using Sharporum.API.Contracts.V1;
using Sharporum.API.Filters;
using Sharporum.API.Helpers;
using Sharporum.Core.Commands.Comment;
using Sharporum.Core.Dtos.Comment;
using Sharporum.Core.Helpers;
using Sharporum.Core.Queries.Comment;
using Sharporum.Core.Responses;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Comment;

namespace Sharporum.API.Controllers.V1
{
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly HttpContext _httpContext;
        private readonly IMediator _mediator;

        public CommentsController(IHttpContextAccessor httpContextAccessor,
            IAuthorizationService authorizationService, IMediator mediator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _authorizationService = authorizationService;
            _mediator = mediator;
        }

        /// <summary>
        ///     Returns comments
        /// </summary>
        /// <param name="searchParams"></param>
        /// <response code="200">Returns comments</response>
        /// <response code="404">Unable to find user with provided "UserId" / post with provided "PostId"</response>
        [HttpGet(ApiRoutes.Comments.GetMany)]
        [Cached(60)]
        [ProducesResponseType(typeof(FilteredResponse<CommentViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] CommentSearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out ErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            FilteredDataViewModel<CommentViewModel> result = await _mediator.Send(new GetCommentsQuery(searchParams));

            return Ok(new FilteredResponse<CommentViewModel>(searchParams)
            {
                Data = result.Data,
                Count = result.Count,
            });
        }

        /// <summary>
        ///     Creates comment
        /// </summary>
        /// <param name="createCommentDto"></param>
        /// <response code="200">Creates comment</response>
        /// <response code="400">Unable to create comment due to validation errors</response>
        /// <response code="404">
        ///     Unable to find user with provided "AuthorId"/ post with provided "PostId" / comment with provided
        ///     "ParentId"
        /// </response>
        [HttpPost(ApiRoutes.Comments.Create)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto createCommentDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            string commentId = await _mediator.Send(new CreateCommentCommand(userId, createCommentDto));

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{commentId}", null);
        }

        /// <summary>
        ///     Returns comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <response code="200">Returns comment</response>
        /// <response code="404">Unable to find comment with provided "commentId"</response>
        [HttpGet(ApiRoutes.Comments.Get)]
        [Cached(60)]
        [ProducesResponseType(typeof(Response<CommentViewModel>), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string commentId)
        {
            return Ok(new Response<CommentViewModel>
            {
                Data = await _mediator.Send(new GetCommentQuery(commentId)),
            });
        }

        /// <summary>
        ///     Updates comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="updateCommentDto"></param>
        /// <response code="200">Updates comment</response>
        /// <response code="400">Unable to update comment due to validation errors</response>
        /// <response code="404">Unable to find comment with provided "commentId"</response>
        [HttpPut(ApiRoutes.Comments.Update)]
        [ProducesResponseType(typeof(Response<CommentViewModel>), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update([FromRoute] string commentId,
            [FromBody] UpdateCommentDto updateCommentDto)
        {
            Comment comment = await _mediator.Send(new GetCommentEntityQuery(commentId));

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, comment, PolicyConstants.UpdateCommentRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            return Ok(new Response<CommentViewModel>
            {
                Data = await _mediator.Send(new UpdateCommentCommand(comment, updateCommentDto)),
            });
        }

        /// <summary>
        ///     Deletes comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <response code="200">Deletes comment</response>
        [HttpDelete(ApiRoutes.Comments.Delete)]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Delete([FromRoute] string commentId)
        {
            Comment comment = await _mediator.Send(new GetCommentEntityQuery(commentId));

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, comment, PolicyConstants.UpdateCommentRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            await _mediator.Send(new DeleteCommentCommand(comment));

            return Ok();
        }

        /// <summary>
        ///     Votes comment
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="commentVoteDto"></param>
        /// <response code="200">Votes comment</response>
        /// <response code="422">Unable to vote comment due to validation errors</response>
        [HttpPost(ApiRoutes.Comments.Vote)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.UnprocessableEntity)]
        public async Task<IActionResult> Vote([FromRoute] string commentId, [FromBody] CommentVoteDto commentVoteDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            await _mediator.Send(new VoteCommentCommand(userId, commentId, commentVoteDto));

            return Ok();
        }
    }
}