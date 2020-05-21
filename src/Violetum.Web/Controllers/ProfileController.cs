using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Web.Models;

namespace Violetum.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IFollowerService _followerService;
        private readonly IPostService _postService;
        private readonly IProfileService _profileService;
        private readonly ITokenManager _tokenManager;

        public ProfileController(IProfileService profileService, ITokenManager tokenManager,
            IPostService postService, IFollowerService followerService)
        {
            _profileService = profileService;
            _tokenManager = tokenManager;
            _postService = postService;
            _followerService = followerService;
        }

        [HttpGet("Profile/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            ProfileViewModel profile = await _profileService.GetProfile(id);

            IEnumerable<PostViewModel> posts = await _postService.GetPosts(new SearchParams
            {
                UserId = profile.Id,
            }, new Paginator());


            string userId = await _tokenManager.GetUserIdFromAccessToken();
            ViewData["UserId"] = userId;

            return View(new ProfilePageViewModel
            {
                Profile = profile,
                Posts = posts,
                IsAuthenticatedUserFollower = _followerService.IsAuthenticatedUserFollower(profile.Id, userId),
            });
        }

        [Authorize]
        [HttpGet("Profile/Edit")]
        public async Task<IActionResult> Edit()
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            ProfileViewModel profile = await _profileService.GetProfile(userId);
            return View(profile);
        }

        [Authorize]
        [HttpPost("Profile/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,Name,GivenName,FamilyName,Picture,Gender,Birthdate,Website")]
            UpdateProfileDto updateProfileDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();

            ProfileViewModel profile = await _profileService.UpdateProfile(userId, updateProfileDto);
            return RedirectToAction(nameof(Index), new
            {
                profile.Id,
            });
        }

        [HttpGet("Profile/{id}/Followers")]
        public async Task<IActionResult> Followers(string id)
        {
            UserFollowersViewModel userFollowers = await _followerService.GetUserFollowers(id);

            return View(userFollowers);
        }

        [HttpGet("Profile/{id}/Following")]
        public async Task<IActionResult> Following(string id)
        {
            UserFollowingViewModel userFollowing = await _followerService.GetUserFollowing(id);

            return View(userFollowing);
        }

        [Authorize]
        [HttpPost("Profile/{id}/Follow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Follow(string id, [Bind("UserToFollowId,FollowerUserId")]
            FollowerDto followerDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            try
            {
                if ((userId != followerDto.FollowerUserId) || (id != followerDto.UserToFollowId))
                {
                    throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
                }

                await _followerService.FollowUser(followerDto);

                return RedirectToAction(nameof(Index), new {Id = id});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Authorize]
        [HttpPost("Profile/{id}/Unfollow")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfollow([Bind("UserToFollowId,FollowerUserId")]
            FollowerDto followerDto)
        {
            string userId = await _tokenManager.GetUserIdFromAccessToken();
            try
            {
                if (userId != followerDto.FollowerUserId)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
                }

                await _followerService.UnfollowUser(followerDto);

                return RedirectToAction(nameof(Index), new {Id = followerDto.UserToFollowId});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }
    }
}