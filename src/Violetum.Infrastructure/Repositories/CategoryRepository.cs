using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public TResult GetCategory<TResult>(Expression<Func<Category, bool>> condition,
            Func<Category, TResult> selector)
        {
            return _context.Categories
                .Include(x => x.Author)
                .Where(condition)
                .Select(selector)
                .FirstOrDefault();
        }

        public IEnumerable<TResult> GetCategories<TResult>(Func<Category, bool> condition,
            Func<Category, TResult> selector, CategorySearchParams searchParams)
        {
            return _context.Categories
                .Include(x => x.Author)
                .Where(condition)
                .Skip(searchParams.Offset)
                .Take(searchParams.Limit)
                .AsEnumerable()
                .Select(selector)
                .ToList();
        }

        public int GetTotalCommentsCount(Func<Category, bool> condition)
        {
            return _context.Categories.Where(condition).Count();
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
            return DeleteEntity(category);
        }
    }
}