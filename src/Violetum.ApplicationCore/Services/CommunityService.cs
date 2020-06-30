using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly ICommunityValidators _communityValidators;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserValidators _userValidators;

        public CommunityService(ICommunityRepository communityRepository, IMapper mapper,
            ICommunityValidators communityValidators, IUserValidators userValidators,
            RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _communityRepository = communityRepository;
            _mapper = mapper;
            _communityValidators = communityValidators;
            _userValidators = userValidators;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public CommunityViewModel GetCommunityById(string communityId)
        {
            return _communityValidators.GetCommunityOrThrow<CommunityViewModel>(x => x.Id == communityId);
        }

        public CommunityViewModel GetCommunityByName(string communityName)
        {
            return _communityValidators.GetCommunityOrThrow<CommunityViewModel>(x => x.Name == communityName);
        }

        public Community GetCommunityEntity(string communityId)
        {
            return _communityValidators.GetCommunityOrThrow(x => x.Id == communityId);
        }

        public async Task<IEnumerable<CommunityViewModel>> GetCommunitiesAsync(CommunitySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrowAsync(searchParams.UserId);
            }

            return _communityRepository.GetCommunities<CommunityViewModel>(searchParams,
                CommunityHelpers.GetCommunityMapperConfiguration());
        }

        public async Task<int> GetCategoriesCountAsync(CommunitySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                await _userValidators.GetUserByIdOrThrowAsync(searchParams.UserId);
            }

            return _communityRepository.GetCommunityCount(searchParams);
        }

        public async Task<string> CreateCommunityAsync(string userId, CreateCommunityDto createCommunityDto)
        {
            User user = await _userValidators.GetUserByIdOrThrowAsync(userId);

            var community = _mapper.Map<Community>(createCommunityDto);
            community.Author = user;

            await _communityRepository.CreateCommunityAsync(community);

            await CreateCommunityAdminRoleAsync(user, community.Id);

            return community.Id;
        }

        public async Task<CommunityViewModel> UpdateCommunityAsync(Community community,
            UpdateCommunityDto updateCommunityDto)
        {
            community.Name = updateCommunityDto.Name;
            community.Description = updateCommunityDto.Description;

            await _communityRepository.UpdateCommunityAsync(community);

            return _mapper.Map<CommunityViewModel>(community);
        }

        public async Task<CommunityViewModel> UpdateCommunityImageAsync(Community community,
            UpdateCommunityImageDto updateCommunityImageDto)
        {
            community.Image = updateCommunityImageDto.Image;

            await _communityRepository.UpdateCommunityAsync(community);

            return _mapper.Map<CommunityViewModel>(community);
        }

        public async Task DeleteCommunityAsync(Community community)
        {
            await RemoveCommunityRolesAsync(community.Id);

            await _communityRepository.DeleteCommunityAsync(community);
        }

        public async Task AddModeratorAsync(Community community, AddModeratorDto addModeratorDto)
        {
            string roleName = $"{nameof(Community)}/{community.Id}/{Roles.Moderator}";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest);
            }

            User newModerator = await _userManager.FindByIdAsync(addModeratorDto.NewModeratorId);
            await _userManager.AddToRoleAsync(newModerator, roleName);
        }

        private async Task CreateCommunityAdminRoleAsync(User user, string communityId)
        {
            string roleName = $"{nameof(Community)}/{communityId}/{Roles.Admin}";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            else
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest);
            }

            await _userManager.AddToRoleAsync(user, roleName);
        }

        private async Task RemoveCommunityRolesAsync(string communityId)
        {
            string roleBase = $"{nameof(Community)}/{communityId}";

            var roles = new List<string>
            {
                $"{roleBase}/{Roles.Admin}",
                $"{roleBase}/{Roles.Moderator}",
            };

            foreach (string roleName in roles)
            {
                IList<User> roleUsers = await _userManager.GetUsersInRoleAsync(roleName);
                foreach (User user in roleUsers)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }

                IdentityRole role = await _roleManager.FindByNameAsync(roleName);

                if (role == null)
                {
                    continue;
                }

                IdentityResult identityResult = await _roleManager.DeleteAsync(role);

                if (!identityResult.Succeeded)
                {
                    throw new HttpStatusCodeException(HttpStatusCode.BadRequest,
                        $"Something went wrong while removing community:{communityId} roles");
                }
            }
        }
    }
}