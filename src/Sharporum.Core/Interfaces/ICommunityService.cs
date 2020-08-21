using System.Collections.Generic;
using System.Threading.Tasks;
using Sharporum.Core.Dtos.Community;
using Sharporum.Core.ViewModels.Community;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Interfaces
{
    public interface ICommunityService
    {
        Task<CommunityViewModel> GetCommunityByIdAsync(string communityId);
        Task<CommunityViewModel> GetCommunityByNameAsync(string communityName);
        Task<Community> GetCommunityEntity(string communityId);
        Task<IEnumerable<CommunityViewModel>> GetCommunitiesAsync(CommunitySearchParams searchParams);
        Task<int> GetCommunitiesCountAsync(CommunitySearchParams searchParams);

        Task<string> CreateCommunityAsync(string userId, CreateCommunityDto createCommunityDto);

        Task<CommunityViewModel> UpdateCommunityAsync(Community community, UpdateCommunityDto updateCommunityDto);

        Task<CommunityViewModel> UpdateCommunityImageAsync(Community community,
            UpdateCommunityImageDto updateCommunityImageDto);

        Task DeleteCommunityAsync(Community community);
        Task AddModeratorAsync(Community community, AddModeratorDto addModeratorDto);
    }
}