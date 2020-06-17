using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Entities;
using Violetum.Domain.Infrastructure;
using Violetum.Domain.Models;

namespace Violetum.Infrastructure.Repositories
{
    [Repository]
    public class VoteRepository : BaseRepository, IVoteRepository
    {
        private readonly ApplicationDbContext _context;

        public VoteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public TEntityVote GetEntityVote<TEntityVote>(Expression<Func<TEntityVote, bool>> condition,
            Func<TEntityVote, TEntityVote> selector) where TEntityVote : class
        {
            return _context.Set<TEntityVote>().Where(condition).FirstOrDefault();
        }

        public Task<int> VoteEntity<TEntityVote>(TEntityVote entityVote) where TEntityVote : class
        {
            return CreateEntity(entityVote);
        }

        public Task<int> UpdateEntityVote<TEntityVote>(TEntityVote entityVote) where TEntityVote : class
        {
            return UpdateEntity(entityVote);
        }
    }
}