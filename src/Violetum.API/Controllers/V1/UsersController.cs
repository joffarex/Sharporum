using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IBlobService _blobService;
        private readonly IFollowerService _followerService;
        private readonly HttpContext _httpContext;
        private readonly IUserService _userService;

        public UsersController(IUserService userService, IFollowerService followerService,
            IBlobService blobService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _followerService = followerService;
            _blobService = blobService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        ///     Returns user
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns user</response>
        /// <response code="404">Unable to find user with provided "userId"</response>
        [HttpGet(ApiRoutes.Users.Get)]
        [ProducesResponseType(typeof(UserResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string userId)
        {
            return Ok(new UserResponse {User = await _userService.GetUser(userId)});
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

            UserViewModel user = await _userService.UpdateUser(userId, updateUserDto);

            return Ok(new UserResponse {User = user});
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

            FileData data = BaseHelpers.GetFileData<User>(updateUserImageDto.Image, userId);
            await _blobService.UploadImageBlob(data.Content, data.FileName);
            updateUserImageDto.Image = data.FileName;

            UserViewModel user = await _userService.UpdateUserImage(userId, updateUserImageDto);

            return Ok(new UserResponse {User = user});
        }

        /// <summary>
        ///     Returns a list of user's followers
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns a list of user's followers</response>
        /// <response code="404">Unable to find user to return followers for</response>
        [HttpGet(ApiRoutes.Users.GetFollowers)]
        [ProducesResponseType(typeof(FollowersResponse<UserFollowersViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFollowers([FromRoute] string userId)
        {
            return Ok(new FollowersResponse<UserFollowersViewModel>
            {
                Followers = await _followerService.GetUserFollowers(userId),
            });
        }

        /// <summary>
        ///     Returns a list of users who current user is following
        /// </summary>
        /// <param name="userId"></param>
        /// <response code="200">Returns a list of users who current user is following</response>
        /// <response code="404">Unable to find user to return following users for</response>
        [HttpGet(ApiRoutes.Users.GetFollowing)]
        [ProducesResponseType(typeof(FollowersResponse<UserFollowingViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFollowing([FromRoute] string userId)
        {
            return Ok(new FollowersResponse<UserFollowingViewModel>
            {
                Followers = await _followerService.GetUserFollowing(userId),
            });
        }

        /// <summary>
        ///     Follows user
        /// </summary>
        /// <param name="userToFollowId"></param>
        /// <response code="200">Follows user</response>
        /// <response code="404">Unable to find user to follow or user who follows</response>
        [HttpPost(ApiRoutes.Users.Follow)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Follow([FromRoute] string userToFollowId)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            await _followerService.FollowUser(userId, userToFollowId);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }

        /// <summary>
        ///     Unfollows user
        /// </summary>
        /// <param name="userToUnfollowId"></param>
        /// <response code="200">Unfollows user</response>
        /// <response code="404">Unable to find user to unfollow or user who unfollows</response>
        [HttpPost(ApiRoutes.Users.Unfollow)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(ActionSuccessResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Unfollow([FromRoute] string userToUnfollowId)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            await _followerService.UnfollowUser(userId, userToUnfollowId);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }
    }
}