﻿using System;
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

        public async Task VoteEntityAsync<TEntityVote>(TEntityVote entityVote) where TEntityVote : class
        {
            await CreateEntityAsync(entityVote);
        }

        public async Task UpdateEntityVoteAsync<TEntityVote>(TEntityVote entityVote) where TEntityVote : class
        {
            await UpdateEntityAsync(entityVote);
        }
    }
}