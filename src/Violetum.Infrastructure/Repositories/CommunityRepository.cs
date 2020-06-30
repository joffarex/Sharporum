using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Infrastructure.Repositories
{
    [Repository]
    public class CommunityRepository : BaseRepository, ICommunityRepository
    {
        private readonly ApplicationDbContext _context;

        public CommunityRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TResult GetCommunity<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return _context.Communities
                .Include(x => x.Author)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefault();
        }

        public Community GetCommunity(Expression<Func<Community, bool>> condition)
        {
            return _context.Communities.Include(x => x.Author).Where(condition).FirstOrDefault();
        }

        public IEnumerable<TResult> GetCommunities<TResult>(CommunitySearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IIncludableQueryable<Community, User> query = _context.Communities
                .Include(x => x.Author);

            IQueryable<Community> whereParams = WhereConditionPredicate(query, searchParams);

            return whereParams
                .ProjectTo<TResult>(configurationProvider)
                .Skip(searchParams.Offset)
                .Take(searchParams.Limit)
                .ToList();
        }

        public int GetCommunityCount(CommunitySearchParams searchParams)
        {
            DbSet<Community> query = _context.Communities;
            IQueryable<Community> whereParams = WhereConditionPredicate(query, searchParams);
            return whereParams.Count();
        }

        public async Task CreateCommunityAsync(Community community)
        {
            await CreateEntityAsync(community);
        }

        public async Task UpdateCommunityAsync(Community community)
        {
            await UpdateEntityAsync(community);
        }

        public async Task DeleteCommunityAsync(Community community)
        {
            List<Post> posts = _context.Posts.Where(x => x.CommunityId == community.Id).ToList();
            if (posts.Any())
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity,
                    "Can not delete category while there are still posts in it");
            }

            await DeleteEntityAsync(community);
        }

        private static IQueryable<Community> WhereConditionPredicate(IQueryable<Community> query,
            CommunitySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                query = query.Include(x => x.Categories.Where(y => y.Name.Contains(searchParams.CategoryName)));
            }

            if (!string.IsNullOrEmpty(searchParams.CommunityName))
            {
                query = query.Where(c => c.Name.Contains(searchParams.CommunityName));
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                query = query.Where(c => c.AuthorId == searchParams.UserId);
            }

            return query;
        }
    }
}