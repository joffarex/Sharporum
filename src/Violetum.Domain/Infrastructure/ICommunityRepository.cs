using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.Domain.Entities;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Domain.Infrastructure
{
    public interface ICommunityRepository
    {
        TResult GetCommunity<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider)
            where TResult : class;

        Community GetCommunity(Expression<Func<Community, bool>> condition);

        IEnumerable<TResult> GetCommunities<TResult>(CommunitySearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class;

        int GetCommunityCount(CommunitySearchParams searchParams);

        Task CreateCommunityAsync(Community community);
        Task UpdateCommunityAsync(Community community);
        Task DeleteCommunityAsync(Community community);
    }
}