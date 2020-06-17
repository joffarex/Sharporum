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
                .Include(x => x.Author)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefault();
        }

        public Category GetCategory(Expression<Func<Category, bool>> condition)
        {
            return _context.Categories.Include(x => x.Author).Where(condition).FirstOrDefault();
        }

        public IEnumerable<TResult> GetCategories<TResult>(CategorySearchParams searchParams,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IIncludableQueryable<Category, User> query = _context.Categories
                .Include(x => x.Author);

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

        public Task<int> CreateCategory(Category category)
        {
            return CreateEntity(category);
        }

        public Task<int> UpdateCategory(Category category)
        {
            return UpdateEntity(category);
        }

        public Task<int> DeleteCategory(Category category)
        {
            var posts = _context.Posts.Where(x => x.CategoryId == category.Id).ToList();
            if (posts.Any())
            {
                throw new HttpStatusCodeException(HttpStatusCode.UnprocessableEntity,
                    $"Can not delete category while there are still posts in it");
            }

            return DeleteEntity(category);
        }

        private static IQueryable<Category> WhereConditionPredicate(IQueryable<Category> query,
            CategorySearchParams searchParams)
        {
            if (!string.IsNullOrEmpty(searchParams.CategoryName))
            {
                query = query.Where(c => c.Name.Contains(searchParams.CategoryName));
            }

            if (!string.IsNullOrEmpty(searchParams.UserId))
            {
                query = query.Where(c => c.AuthorId == searchParams.UserId);
            }

            return query;
        }
    }
}