using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Violetum.Domain.Infrastructure;

namespace Violetum.Infrastructure.Repositories
{
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
            return _context.Set<TEntityVote>().Where(condition).Select(selector).FirstOrDefault();
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