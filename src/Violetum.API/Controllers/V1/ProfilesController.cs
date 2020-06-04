using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.API.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.CustomExceptions;

namespace Violetum.API.Controllers.V1
{
    public class ProfilesController : ControllerBase
    {
        private readonly IFollowerService _followerService;
        private readonly HttpContext _httpContext;
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService, IFollowerService followerService,
            IHttpContextAccessor httpContextAccessor)
        {
            _profileService = profileService;
            _followerService = followerService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        [HttpGet(ApiRoutes.Profiles.Get)]
        public async Task<IActionResult> Get([FromRoute] string profileId)
        {
            return Ok(new ProfileResponse {Profile = await _profileService.GetProfile(profileId)});
        }

        [HttpPut(ApiRoutes.Profiles.Update)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update([FromRoute] string profileId,
            [FromBody] UpdateProfileDto updateProfileDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            if (profileId != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            ProfileViewModel profile = await _profileService.UpdateProfile(userId, updateProfileDto);

            return Ok(new ProfileResponse {Profile = profile});
        }

        [HttpGet(ApiRoutes.Profiles.GetFollowers)]
        public async Task<IActionResult> GetFollowers([FromRoute] string profileId)
        {
            return Ok(new FollowersResponse<UserFollowersViewModel>
            {
                Followers = await _followerService.GetUserFollowers(profileId),
            });
        }

        [HttpGet(ApiRoutes.Profiles.GetFollowing)]
        public async Task<IActionResult> GetFollowing([FromRoute] string profileId)
        {
            return Ok(new FollowersResponse<UserFollowingViewModel>
            {
                Followers = await _followerService.GetUserFollowing(profileId),
            });
        }

        [HttpPost(ApiRoutes.Profiles.Follow)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Follow([FromQuery] string profileId,
            [FromBody] FollowActionDto followActionDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            if ((userId != followActionDto.FollowerUserId) || (profileId != followActionDto.UserToFollowId))
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            await _followerService.FollowUser(followActionDto);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }

        [HttpPost(ApiRoutes.Profiles.Unfollow)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Unfollow([FromQuery] string profileId,
            [FromBody] FollowActionDto followActionDto)
        {
            string userId = _httpContext.User.FindFirstValue("sub");

            if ((userId != followActionDto.FollowerUserId) || (profileId != followActionDto.UserToFollowId))
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            await _followerService.UnfollowUser(followActionDto);

            return Ok(new ActionSuccessResponse {Message = "OK"});
        }
    }
}