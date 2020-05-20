using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Category GetCategory(Expression<Func<Category, bool>> condition)
        {
            return _context.Categories
                .Include(x => x.Author)
                .FirstOrDefault(condition);
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
            Func<Category, TResult> selector, Paginator paginator)
        {
            return _context.Categories
                .Include(x => x.Author)
                .AsEnumerable()
                .Where(condition)
                .Select(selector)
                .Skip(paginator.Offset)
                .Take(paginator.Limit)
                .ToList();
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