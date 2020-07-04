using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.API.Filters;
using Violetum.ApplicationCore.Commands.User;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.Queries.User;
using Violetum.ApplicationCore.Responses;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.Domain.Models;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly HttpContext _httpContext;
        private readonly IMediator _mediator;

        public UsersController(IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            _mediator = mediator;
            _httpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        ///     Returns user
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns user</response>
        /// <response code="404">Unable to find user with provided "userId"</response>
        [HttpGet(ApiRoutes.Users.Get)]
        [Cached(120)]
        [ProducesResponseType(typeof(UserResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string userId)
        {
            var query = new GetUserQuery(userId);
            UserResponse result = await _mediator.Send(query);

            return Ok(result);
        }

        /// <summary>
        ///     Updates user
        /// </summary>
        /// <param name="updateUserDto"></param>
        /// <response code="200">Updates user</response>
        /// <response code="400">Unable to update user due to validation errors</response>
        [HttpPut(ApiRoutes.Users.Update)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(UserResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(
            [FromBody] UpdateUserDto updateUserDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new UpdateUserCommand(userId, updateUserDto);
            UserResponse result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        ///     Updates user's image
        /// </summary>
        /// <param name="updateUserImageDto"></param>
        /// <response code="200">Updates user's image</response>
        /// <response code="400">Unable to update user due to validation errors</response>
        [HttpPut(ApiRoutes.Users.UpdateImage)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(UserResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateImage([FromBody] UpdateUserImageDto updateUserImageDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new UpdateUserImageCommand(userId, updateUserImageDto);
            UserResponse result = await _mediator.Send(command);

            return Ok(result);
        }

        /// <summary>
        ///     Returns a list of user's followers
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns a list of user's followers</response>
        /// <response code="404">Unable to find user to return followers for</response>
        [HttpGet(ApiRoutes.Users.GetFollowers)]
        [ProducesResponseType(typeof(Response<UserFollowersViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFollowers([FromRoute] string userId)
        {
            var query = new GetFollowersQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(new Response<UserFollowersViewModel> {Data = result});
        }

        /// <summary>
        ///     Returns a list of users who current user is following
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns a list of users who current user is following</response>
        /// <response code="404">Unable to find user to return following users for</response>
        [HttpGet(ApiRoutes.Users.GetFollowing)]
        [ProducesResponseType(typeof(Response<UserFollowingViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFollowing([FromRoute] string userId)
        {
            var query = new GetFollowingQuery(userId);
            var result = await _mediator.Send(query);

            return Ok(new Response<UserFollowingViewModel> {Data = result});
        }

        /// <summary>
        ///     Follows user
        /// </summary>
        /// <param name="userToFollowId"></param>
        /// <response code="200">Follows user</response>
        /// <response code="404">Unable to find user to follow or user who follows</response>
        [HttpPost(ApiRoutes.Users.Follow)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Follow([FromRoute] string userToFollowId)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new FollowUserCommand(userId, userToFollowId);
            await _mediator.Send(command);

            return Ok();
        }

        /// <summary>
        ///     Unfollows user
        /// </summary>
        /// <param name="userToUnfollowId"></param>
        /// <response code="200">Unfollows user</response>
        /// <response code="404">Unable to find user to unfollow or user who unfollows</response>
        [HttpPost(ApiRoutes.Users.Unfollow)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Unfollow([FromRoute] string userToUnfollowId)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            var command = new UnfollowUserCommand(userId, userToUnfollowId);
            await _mediator.Send(command);

            return Ok();
        }

        /// <summary>
        ///     Returns User's by post rank
        /// </summary>
        /// <response code="200">Returns User's by post rank</response>
        [HttpGet(ApiRoutes.Users.PostRanks)]
        [ProducesResponseType(typeof(Response<IEnumerable<Ranks>>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> PostRanks()
        {
            var query = new GetPostRanksQuery();
            var result = await _mediator.Send(query);

            return Ok(new Response<IEnumerable<Ranks>> {Data = result});
        }

        /// <summary>
        ///     Returns User's by comment rank
        /// </summary>
        /// <response code="200">Returns User's by comment rank</response>
        [HttpGet(ApiRoutes.Users.CommentRanks)]
        [ProducesResponseType(typeof(Response<IEnumerable<Ranks>>), (int) HttpStatusCode.OK)]
        public async Task<IActionResult> CommentRanks()
        {
            var query = new GetCommentRanksQuery();
            var result = await _mediator.Send(query);

            return Ok(new Response<IEnumerable<Ranks>> {Data = result});
        }
    }
}