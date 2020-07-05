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
    public class CommunityRepository : IAsyncRepository<Community>
    {
        private readonly ApplicationDbContext _context;

        public CommunityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Communities
                .Include(x => x.Author)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<Community> GetByConditionAsync(Expression<Func<Community, bool>> condition)
        {
            return await _context.Communities.Include(x => x.Author).Where(condition).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISearchParams<Community> searchParams,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IIncludableQueryable<Community, User> query = _context.Communities
                .Include(x => x.Author);

            // IQueryable<Community> whereParams = WhereConditionPredicate(query, searchParams);

            return await query
                .ProjectTo<TResult>(configurationProvider)
                .Skip(searchParams.Offset)
                .Take(searchParams.Limit)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(ISearchParams<Community> searchParams)
        {
            DbSet<Community> query = _context.Communities;
            // IQueryable<Community> whereParams = WhereConditionPredicate(query, searchParams);
            return await query.CountAsync();
        }

        public async Task CreateAsync(Community community)
        {
            await _context.Communities.AddAsync(community);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Community community)
        {
            _context.Communities.Update(community);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Community community)
        {
            List<Post> posts = await _context.Posts.Where(x => x.CommunityId == community.Id).ToListAsync();
            if (posts.Any())
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity,
                    "Can not delete category while there are still posts in it");
            }

            _context.Communities.Remove(community);

            await _context.SaveChangesAsync();
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