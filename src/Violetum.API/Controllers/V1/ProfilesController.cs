using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Violetum.ApplicationCore.Contracts.V1;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;

namespace Violetum.API.Controllers.V1
{
    [Produces("application/json")]
    public class ProfilesController : ControllerBase
    {
        private readonly IBlobService _blobService;
        private readonly IFollowerService _followerService;
        private readonly HttpContext _httpContext;
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService, IFollowerService followerService,
            IBlobService blobService,
            IHttpContextAccessor httpContextAccessor)
        {
            _profileService = profileService;
            _followerService = followerService;
            _blobService = blobService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        /// <summary>
        ///     Returns user's profile
        /// </summary>
        /// <param name="profileId"></param>
        /// <response code="200">Returns user's profile</response>
        /// <response code="404">Unable to find profile with provided "profileId"</response>
        [HttpGet(ApiRoutes.Profiles.Get)]
        [ProducesResponseType(typeof(ProfileResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromRoute] string profileId)
        {
            return Ok(new ProfileResponse {Profile = await _profileService.GetProfile(profileId)});
        }

        /// <summary>
        ///     Updates user's profile page
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="updateProfileDto"></param>
        /// <response code="200">Updates user's profile</response>
        /// <response code="400">Unable to update user's profile due to validation errors</response>
        [HttpPut(ApiRoutes.Profiles.Update)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(ProfileResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update([FromRoute] string profileId,
            [FromBody] UpdateProfileDto updateProfileDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            if (profileId != userId)
            {
                return new UnauthorizedResult();
            }

            ProfileViewModel profile = await _profileService.UpdateProfile(userId, updateProfileDto);

            return Ok(new ProfileResponse {Profile = profile});
        }

        /// <summary>
        ///     Updates user's profile image
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="updateProfileImageDto"></param>
        /// <response code="200">Updates user's profile image</response>
        /// <response code="400">Unable to update user's profile due to validation errors</response>
        [HttpPut(ApiRoutes.Profiles.UpdateImage)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ProducesResponseType(typeof(ProfileResponse), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateImage([FromRoute] string profileId,
            [FromBody] UpdateProfileImageDto updateProfileImageDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            if (profileId != userId)
            {
                return new UnauthorizedResult();
            }

            FileData data = BaseHelpers.GetFileData<User>(updateProfileImageDto.Image, userId);
            await _blobService.UploadImageBlob(data.Content, data.FileName);
            updateProfileImageDto.Image = data.FileName;

            ProfileViewModel profile = await _profileService.UpdateProfileImage(userId, updateProfileImageDto);

            return Ok(new ProfileResponse {Profile = profile});
        }

        /// <summary>
        ///     Returns a list of user's followers
        /// </summary>
        /// <param name="profileId"></param>
        /// <response code="200">Returns a list of user's followers</response>
        /// <response code="404">Unable to find user to return followers for</response>
        [HttpGet(ApiRoutes.Profiles.GetFollowers)]
        [ProducesResponseType(typeof(FollowersResponse<UserFollowersViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFollowers([FromRoute] string profileId)
        {
            return Ok(new FollowersResponse<UserFollowersViewModel>
            {
                Followers = await _followerService.GetUserFollowers(profileId),
            });
        }

        /// <summary>
        ///     Returns a list of users who current user is following
        /// </summary>
        /// <param name="profileId"></param>
        /// <response code="200">Returns a list of users who current user is following</response>
        /// <response code="404">Unable to find user to return following users for</response>
        [HttpGet(ApiRoutes.Profiles.GetFollowing)]
        [ProducesResponseType(typeof(FollowersResponse<UserFollowingViewModel>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorDetails), (int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetFollowing([FromRoute] string profileId)
        {
            return Ok(new FollowersResponse<UserFollowingViewModel>
            {
                Followers = await _followerService.GetUserFollowing(profileId),
            });
        }

        /// <summary>
        ///     Follows user
        /// </summary>
        /// <param name="userToFollowId"></param>
        /// <response code="200">Follows user</response>
        /// <response code="404">Unable to find user to follow or user who follows</response>
        [HttpPost(ApiRoutes.Profiles.Follow)]
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
        [HttpPost(ApiRoutes.Profiles.Unfollow)]
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