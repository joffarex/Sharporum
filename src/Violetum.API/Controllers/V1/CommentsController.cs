using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Authorization;
using Violetum.API.Filters;
using Violetum.API.Helpers;
using Violetum.ApplicationCore.Commands.Comment;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Queries.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.API.Controllers.V1
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
        [ProducesResponseType(typeof(GetManyResponse<CommentViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetMany([FromQuery] CommentSearchParams searchParams)
        {
            if (!BaseHelpers.IsPaginatonSearchParamsValid(searchParams, out QueryStringErrorResponse errorResponse))
            {
                return new BadRequestObjectResult(errorResponse);
            }

            var query = new GetCommentsQuery(searchParams);
            GetManyResponse<CommentViewModel> result = await _mediator.Send(query);

            return Ok(result);
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
        [ProducesResponseType(typeof(CreatedResponse), (int) HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Create([FromBody] CreateCommentDto createCommentDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new CreateCommentCommand(userId, createCommentDto);
            CreatedResponse result = await _mediator.Send(command);

            return Created($"{HttpContext.Request.GetDisplayUrl()}/{result.Id}", result);
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
            var query = new GetCommentQuery(commentId);
            CommentViewModel result = await _mediator.Send(query);

            return Ok(new Response<CommentViewModel> {Data = result});
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
            var query = new GetCommentEntityQuery(commentId);
            Comment comment = await _mediator.Send(query);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, comment, PolicyConstants.UpdateCommentRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            var command = new UpdateCommentCommand(comment, updateCommentDto);
            CommentViewModel result = await _mediator.Send(command);

            return Ok(new Response<CommentViewModel> {Data = result});
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
            var query = new GetCommentEntityQuery(commentId);
            Comment comment = await _mediator.Send(query);

            AuthorizationResult authorizationResult =
                await _authorizationService.AuthorizeAsync(User, comment, PolicyConstants.UpdateCommentRolePolicy);

            if (!authorizationResult.Succeeded)
            {
                return ActionResults.UnauthorizedResult(User.Identity.IsAuthenticated);
            }

            var command = new DeleteCommentCommand(comment);
            await _mediator.Send(command);

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

            var command = new VoteCommentCommand(userId, commentId, commentVoteDto);
            await _mediator.Send(command);

            return Ok();
        }
    }
}