using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.CustomExceptions;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.Infrastructure.Repositories
{
    [Repository]
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TResult GetCategory<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return _context.Categories
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefault();
        }

        public Category GetCategory(Expression<Func<Category, bool>> condition)
        {
            return _context.Categories.Where(condition).FirstOrDefault();
        }

        public IEnumerable<TResult> GetCategories<TResult>(CategorySearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            DbSet<Category> query = _context.Categories;

            IQueryable<Category> whereParams = WhereConditionPredicate(query, searchParams);

            return whereParams
                .ProjectTo<TResult>(configurationProvider)
                .Skip(searchParams.Offset)
                .Take(searchParams.Limit)
                .ToList();
        }

        public int GetCategoryCount(CategorySearchParams searchParams)
        {
            DbSet<Category> query = _context.Categories;
            IQueryable<Category> whereParams = WhereConditionPredicate(query, searchParams);
            return whereParams.Count();
        }

        public async Task CreateCategoryAsync(Category category)
        {
            await CreateEntityAsync(category);
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            await UpdateEntityAsync(category);
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            if (_context.CommunityCategories.Where(x => x.CategoryId == category.Id).Any())
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity,
                    "Can not delete category while there are still communities attached to it");
            }

            await DeleteEntityAsync(category);
        }

        private static IQueryable<Category> WhereConditionPredicate(IQueryable<Category> query,
            CategorySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                query = query.Where(c => c.Name.Contains(searchParams.CategoryName));
            }

            return query;
        }
    }
}