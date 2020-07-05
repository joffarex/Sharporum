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
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
    public class CommentRepository : BaseRepository, IAsyncRepository<Comment>
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<TResult> GetByConditionAsync<TResult>(Expression<Func<TResult, bool>> condition,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Comments
                .Include(x => x.Author)
                .Include(x => x.Post)
                .ProjectTo<TResult>(configurationProvider)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<Comment> GetByConditionAsync(Expression<Func<Comment, bool>> condition)
        {
            return await _context.Comments
                .Include(x => x.Post)
                .Include(x => x.Author)
                .Where(condition)
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<Comment> specification,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            IQueryable<TResult> query =
                await ApplySpecification<Comment, TResult>(specification, configurationProvider);

            return await query.ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(ISpecification<Comment> specification)
        {
            IQueryable<Comment> query = await ApplySpecification(specification);

            return await query.CountAsync();
        }

        public async Task CreateAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            IIncludableQueryable<Comment, IEnumerable<CommentVote>> votes =
                _context.Comments.Where(x => x.Id == comment.Id).Include(x => x.CommentVotes);
            _context.Comments.RemoveRange(votes);

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();
        }
    }
}