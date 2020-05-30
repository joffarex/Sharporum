using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Violetum.API.Contracts.V1;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.CustomExceptions;

namespace Violetum.API.Controllers.V1
{
    public class ProfilesController : ControllerBase
    {
        private readonly IFollowerService _followerService;
        private readonly IProfileService _profileService;

        public ProfilesController(IProfileService profileService, IFollowerService followerService)
        {
            _profileService = profileService;
            _followerService = followerService;
        }

        [HttpGet(ApiRoutes.Profiles.Get)]
        public async Task<IActionResult> Get([FromRoute] string profileId)
        {
            return Ok(new {Profile = await _profileService.GetProfile(profileId)});
        }

        [HttpPut(ApiRoutes.Profiles.Update)]
        public async Task<IActionResult> Update([FromRoute] string profileId,
            [FromBody] UpdateProfileDto updateProfileDto)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            if (profileId != userId)
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            ProfileViewModel profile = await _profileService.UpdateProfile(userId, updateProfileDto);

            return Ok(new {Profile = profile});
        }

        [HttpGet(ApiRoutes.Profiles.GetFollowers)]
        public async Task<IActionResult> GetFollowers(string profileId)
        {
            return Ok(new {Followers = await _followerService.GetUserFollowers(profileId)});
        }

        [HttpGet(ApiRoutes.Profiles.GetFollowing)]
        public async Task<IActionResult> GetFollowing(string profileId)
        {
            return Ok(new {Followers = await _followerService.GetUserFollowing(profileId)});
        }

        [HttpPost(ApiRoutes.Profiles.Follow)]
        public async Task<IActionResult> Follow([FromQuery] string profileId,
            [FromBody] FollowActionDto followActionDto)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            if ((userId != followActionDto.FollowerUserId) || (profileId != followActionDto.UserToFollowId))
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            await _followerService.FollowUser(followActionDto);

            return Ok(new {Message = "OK"});
        }

        [HttpPost(ApiRoutes.Profiles.Unfollow)]
        public async Task<IActionResult> Unfollow([FromQuery] string profileId,
            [FromBody] FollowActionDto followActionDto)
        {
            string userId = User.Claims.Where(x => x.Type == "sub").Select(x => x.Value).FirstOrDefault();

            if ((userId != followActionDto.FollowerUserId) || (profileId != followActionDto.UserToFollowId))
            {
                throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
            }

            await _followerService.UnfollowUser(followActionDto);

            return Ok(new {Message = "OK"});
        }
    }
}