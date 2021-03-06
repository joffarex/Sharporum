﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sharporum.Domain.Entities;

namespace Sharporum.Domain.Infrastructure
{
    public interface ICommunityRepository : IAsyncRepository<Community>
    {
        Task<IReadOnlyList<CommunityCategory>> ListCommunityCategoriesAsync(string communityId);
        Task AddCategoriesToCommunityAsync(string communityId, IEnumerable<string> categoryIds);
        Task DeleteCommunityCategoriesAsync(string communityId);
    }
}