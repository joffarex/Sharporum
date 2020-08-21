using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sharporum.Domain.Infrastructure;

namespace Sharporum.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext _context;

        public VoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TEntityVote> GetEntityVoteAsync<TEntityVote>(Expression<Func<TEntityVote, bool>> condition,
            Func<TEntityVote, TEntityVote> selector) where TEntityVote : class
        {
            return await _context.Set<TEntityVote>().Where(condition).FirstOrDefaultAsync();
        }

        public async Task VoteEntityAsync<TEntityVote>(TEntityVote entityVote) where TEntityVote : class
        {
            await _context.Set<TEntityVote>().AddAsync(entityVote);

            await _context.SaveChangesAsync();
        }

        public async Task UpdateEntityVoteAsync<TEntityVote>(TEntityVote entityVote) where TEntityVote : class
        {
            _context.Set<TEntityVote>().Update(entityVote);

            await _context.SaveChangesAsync();
        }
    }
}