using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Joffarex.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Infrastructure;

namespace Sharporum.Infrastructure.Repositories
{
    public class PostRepository : BaseRepository, IAsyncRepository<Post>
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Community)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<Post> GetByConditionAsync(Expression<Func<Post, bool>> condition)
        {
            return await _context.Posts
                .Include(x => x.Author)
                .Include(x => x.Community)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<Post> specification,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IQueryable<TResult> query = await ApplySpecification<Post, TResult>(specification, configurationProvider);

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(ISpecification<Post> specification)
        {
            IQueryable<Post> query = await ApplySpecification(specification);

            return await query.CountAsync();
        }

        public async Task CreateAsync(Post post)
        {
            await _context.Posts.AddAsync(post);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Post post)
        {
            IQueryable<Comment> comments = _context.Comments.Where(x => x.PostId == post.Id);
            _context.Comments.RemoveRange(comments);

            IIncludableQueryable<Post, IEnumerable<PostVote>> votes = _context.Posts.Where(x => x.Id == post.Id)
                .Include(x => x.PostVotes);
            _context.Posts.RemoveRange(votes);

            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();
        }
    }
}