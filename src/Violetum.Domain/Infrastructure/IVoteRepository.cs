using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Violetum.Domain.Infrastructure
{
    public interface IVoteRepository
    {
        Task<TEntityVote> GetEntityVoteAsync<TEntityVote>(Expression<Func<TEntityVote, bool>> condition,
            Func<TEntityVote, TEntityVote> selector) where TEntityVote : class;

        Task VoteEntityAsync<TEntityVote>(TEntityVote entityVote) where TEntityVote : class;
        Task UpdateEntityVoteAsync<TEntityVote>(TEntityVote entityVote) where TEntityVote : class;

        Task<IReadOnlyList<TEntityVote>> GetEntityRanksAsync<TEntityVote>();
        Task<int> GetUserEntityRanksAsync(string userId);
    }
}