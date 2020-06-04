using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;
using Violetum.Web.Models;

namespace Violetum.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IFollowerService _followerService;
        private readonly IIdentityManager _identityManager;
        private readonly IMapper _mapper;
        private readonly IPostService _postService;
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService, IIdentityManager identityManager,
            IPostService postService, IFollowerService followerService, IMapper mapper)
        {
            _profileService = profileService;
            _identityManager = identityManager;
            _postService = postService;
            _followerService = followerService;
            _mapper = mapper;
        }

        [HttpGet("Profile/{id}")]
        public async Task<IActionResult> Index(string id, string postSortBy, string postDir, int postPage)
        {
            ViewData["SortByParm"] = string.IsNullOrEmpty(postSortBy) ? "CreatedAt" : postSortBy;
            ViewData["OrderByDirParm"] = string.IsNullOrEmpty(postDir) ? "desc" : postDir;
            ViewData["CurrentPageParm"] = postPage != 0 ? postPage : 1;

            ProfileViewModel profile = await _profileService.GetProfile(id);

            var searchParams = new PostSearchParams
            {
                SortBy = (string) ViewData["SortByParm"],
                OrderByDir = (string) ViewData["OrderByDirParm"],
                CurrentPage = (int) ViewData["CurrentPageParm"],
                UserId = profile.Id,
            };

            IEnumerable<PostViewModel> posts = await _postService.GetPosts(searchParams);

            var totalPages =
                (int) Math.Ceiling(await _postService.GetTotalPostsCount(searchParams) / (double) searchParams.Limit);
            ViewData["totalPages"] = totalPages;

            string userId = _identityManager.GetUserId();
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
            string userId = _identityManager.GetUserId();

            ProfileViewModel profile = await _profileService.GetProfile(userId);
            return View(_mapper.Map<UpdateProfileDto>(profile));
        }

        [Authorize]
        [HttpPost("Profile/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,Username,Name,GivenName,FamilyName,Picture,Gender,Birthdate,Website")]
            UpdateProfileDto updateProfileDto)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Edit));
            }

            string userId = _identityManager.GetUserId();

            TempData["EditProfileSuccess"] = "Profile successfully edited";
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
            FollowActionDto followActionDto)
        {
            string userId = _identityManager.GetUserId();
            try
            {
                if ((userId != followActionDto.FollowerUserId) || (id != followActionDto.UserToFollowId))
                {
                    throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
                }

                await _followerService.FollowUser(followActionDto);

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
            FollowActionDto followActionDto)
        {
            string userId = _identityManager.GetUserId();
            try
            {
                if (userId != followActionDto.FollowerUserId)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.Unauthorized, $"Unauthorized User:{userId}");
                }

                await _followerService.UnfollowUser(followActionDto);

                return RedirectToAction(nameof(Index), new {Id = followActionDto.UserToFollowId});
            }
            catch (DbUpdateException e)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [Authorize]
        public async Task<IActionResult> NewsFeed(string sortBy, string dir, int page, string title)
        {
            ViewData["SortByParm"] = string.IsNullOrEmpty(sortBy) ? "CreatedAt" : sortBy;
            ViewData["OrderByDirParm"] = string.IsNullOrEmpty(dir) ? "desc" : dir;
            ViewData["CurrentPageParm"] = page != 0 ? page : 1;
            ViewData["PostTitleParm"] = title;

            var searchParams = new PostSearchParams
            {
                SortBy = (string) ViewData["SortByParm"],
                OrderByDir = (string) ViewData["OrderByDirParm"],
                CurrentPage = (int) ViewData["CurrentPageParm"],
            };

            if (!string.IsNullOrEmpty(title))
            {
                searchParams.PostTitle = title;
            }

            string userId = _identityManager.GetUserId();
            ViewData["UserId"] = userId;

            IEnumerable<PostViewModel> posts = _postService.GetNewsFeedPosts(userId, searchParams);
            int totalPosts = _postService.GetTotalPostsCountInNewsFeed(userId, searchParams);
            ViewData["TotalPosts"] = totalPosts;

            var totalPages =
                (int) Math.Ceiling(totalPosts / (double) searchParams.Limit);
            ViewData["totalPages"] = totalPages;


            return View(posts);
        }
    }
}