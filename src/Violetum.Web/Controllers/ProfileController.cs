using System;
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
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            try
            {
                ProfileViewModel profile = await _profileService.GetProfile(id);
                if (profile == null)
                {
                    return BadRequest();
                }

                IEnumerable<PostViewModel> posts = await GetUserPosts(new SearchParams
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpGet("Profile/Edit")]
        public async Task<IActionResult> Edit()
        {
            try
            {
                string userId = await _tokenManager.GetUserIdFromAccessToken();
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest();
                }

                ProfileViewModel profile = await _profileService.GetProfile(userId);
                return View(profile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("Profile/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            [Bind("Id,Name,GivenName,FamilyName,Picture,Gender,Birthdate,Website")]
            UpdateProfileDto updateProfileDto)
        {
            try
            {
                string userId = await _tokenManager.GetUserIdFromAccessToken();

                ProfileViewModel profile = await _profileService.UpdateProfile(userId, updateProfileDto);

                return RedirectToAction(nameof(Index), new {profile.Id});
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
        }

        private async Task<IEnumerable<PostViewModel>> GetUserPosts(SearchParams searchParams, Paginator paginator)
        {
            return await _postService.GetPosts(searchParams, paginator);
        }
    }
}