using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public TResult GetCategoryByName<TResult>(string name, Func<Category, TResult> selector)
        {
            return _context.Categories.Where(x => x.Name == name).Select(selector).FirstOrDefault();
        }

        public IEnumerable<TResult> GetCategories<TResult>(Expression<Func<Category, bool>> predicate,
            Func<Category, TResult> selector, Paginator paginator)
        {
            return GetEntities(predicate, selector, paginator);
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