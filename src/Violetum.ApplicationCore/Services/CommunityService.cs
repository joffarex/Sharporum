using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.Helpers;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Specifications;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;

        public CommunityService(ICommunityRepository communityRepository, RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager, IMapper mapper)
        {
            _communityRepository = communityRepository;
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<CommunityViewModel> GetCommunityByIdAsync(string communityId)
        {
            var community = await _communityRepository.GetByConditionAsync<CommunityViewModel>(x => x.Id == communityId,
                CommunityHelpers.GetCommunityMapperConfiguration());
            Guard.Against.NullItem(community, nameof(community));

            return community;
        }

        public async Task<CommunityViewModel> GetCommunityByNameAsync(string communityName)
        {
            var community = await _communityRepository.GetByConditionAsync<CommunityViewModel>(
                x => x.Name == communityName,
                CommunityHelpers.GetCommunityMapperConfiguration());
            Guard.Against.NullItem(community, nameof(community));

            return community;
        }

        public async Task<Community> GetCommunityEntity(string communityId)
        {
            Community community = await _communityRepository.GetByConditionAsync(x => x.Id == communityId);
            Guard.Against.NullItem(community, nameof(community));

            return community;
        }

        public async Task<IEnumerable<CommunityViewModel>> GetCommunitiesAsync(CommunitySearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user, nameof(user));

            var specification = new CommunityFilterSpecification(searchParams);

            return await _communityRepository.ListAsync<CommunityViewModel>(specification,
                CommunityHelpers.GetCommunityMapperConfiguration());
        }

        public async Task<int> GetCommunitiesCountAsync(CommunitySearchParams searchParams)
        {
            User user = await _userManager.FindByIdAsync(searchParams.UserId);
            Guard.Against.NullItem(user, nameof(user));

            var specification = new CommunityFilterSpecification(searchParams);

            return await _communityRepository.GetTotalCountAsync(specification);
        }

        public async Task<string> CreateCommunityAsync(string userId, CreateCommunityDto createCommunityDto)
        {
            User user = await _userManager.FindByIdAsync(userId);
            Guard.Against.NullItem(user, nameof(user));

            var community = _mapper.Map<Community>(createCommunityDto);
            community.Author = user;

            await _communityRepository.CreateAsync(community);

            if (createCommunityDto.CategoryIds.Any())
            {
                await _communityRepository.AddCategoriesToCommunityAsync(community.Id, createCommunityDto.CategoryIds);
            }

            await CreateCommunityAdminRoleAsync(user, community.Id);

            return community.Id;
        }

        public async Task<CommunityViewModel> UpdateCommunityAsync(Community community,
            UpdateCommunityDto updateCommunityDto)
        {
            community.Name = updateCommunityDto.Name;
            community.Description = updateCommunityDto.Description;

            await _communityRepository.UpdateAsync(community);

            if (updateCommunityDto.CategoryIds.Any())
            {
                await _communityRepository.AddCategoriesToCommunityAsync(community.Id, updateCommunityDto.CategoryIds);
            }

            return _mapper.Map<CommunityViewModel>(community);
        }

        public async Task<CommunityViewModel> UpdateCommunityImageAsync(Community community,
            UpdateCommunityImageDto updateCommunityImageDto)
        {
            community.Image = updateCommunityImageDto.Image;

            await _communityRepository.UpdateAsync(community);

            return _mapper.Map<CommunityViewModel>(community);
        }

        public async Task DeleteCommunityAsync(Community community)
        {
            await RemoveCommunityRolesAsync(community.Id);

            await _communityRepository.DeleteAsync(community);
            await _communityRepository.DeleteCommunityCategoriesAsync(community.Id);
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