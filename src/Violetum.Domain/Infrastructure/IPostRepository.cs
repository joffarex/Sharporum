using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Violetum.Domain.Entities;
using Violetum.Domain.Models;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Domain.Infrastructure
{
    public interface IPostRepository
    {
        TResult GetPost<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider)
            where TResult : class;

        Post GetPost(Expression<Func<Post, bool>> condition);

        IEnumerable<TResult> GetPosts<TResult>(PostSearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class;

        IEnumerable<string> GetUserFollowings(string userId);

        int GetPostCount(PostSearchParams searchParams);

        Task<int> CreatePost(Post post);
        Task<int> UpdatePost(Post post);
        Task<int> DeletePost(Post post);

        List<Ranks> GetPostRanks();
        int GetUserPostRank(string userId);
    }
}