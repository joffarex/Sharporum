﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Community;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface ICommunityService
    {
        CommunityViewModel GetCommunityById(string communityId);
        CommunityViewModel GetCommunityByName(string communityName);
        Community GetCommunityEntity(string communityId);
        Task<IEnumerable<CommunityViewModel>> GetCommunitiesAsync(CommunitySearchParams searchParams);
        Task<int> GetCategoriesCountAsync(CommunitySearchParams searchParams);

        Task<string> CreateCommunityAsync(string userId, CreateCommunityDto createCommunityDto);

        Task<CommunityViewModel> UpdateCommunityAsync(Community community, UpdateCommunityDto updateCommunityDto);

        Task<CommunityViewModel> UpdateCommunityImageAsync(Community community,
            UpdateCommunityImageDto updateCommunityImageDto);

        Task DeleteCommunityAsync(Community community);
        Task AddModeratorAsync(Community community, AddModeratorDto addModeratorDto);
    }
}