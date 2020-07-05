using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Joffarex.Specification;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    [Repository]
    public class CategoryRepository : BaseRepository, IAsyncRepository<Category>
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Categories
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<Category> GetByConditionAsync(Expression<Func<Category, bool>> condition)
        {
            return await _context.Categories.Where(condition).FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<Category> specification,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            var query = await ApplySpecification<Category, TResult>(specification, configurationProvider);

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(ISpecification<Category> specification)
        {
            var query = await ApplySpecification(specification);

            return await query.CountAsync();
        }

        public async Task CreateAsync(Category category)
        {
            await _context.Categories.AddAsync(category);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Category category)
        {
            if (await _context.CommunityCategories.Where(x => x.CategoryId == category.Id).AnyAsync())
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity,
                    "Can not delete category while there are still communities attached to it");
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();
        }
    }
}