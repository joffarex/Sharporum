using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Violetum.Domain.Infrastructure
{
    public interface IVoteRepository
    {
        TEntityVote GetEntityVote<TEntityVote>(Expression<Func<TEntityVote, bool>> condition,
            Func<TEntityVote, TEntityVote> selector) where TEntityVote : class;

        Task<int> VoteEntity<TEntityVote>(TEntityVote entityVote) where TEntityVote : class;
        Task<int> UpdateEntityVote<TEntityVote>(TEntityVote entityVote) where TEntityVote : class;
    }
}