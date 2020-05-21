using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Web.Models;

namespace Violetum.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IPostService _postService;
        private readonly IProfileService _profileService;
        private readonly ITokenManager _tokenManager;

        public ProfileController(IProfileService profileService, ITokenManager tokenManager,
            IPostService postService)
        {
            _profileService = profileService;
            _tokenManager = tokenManager;
            _postService = postService;
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
    }
}