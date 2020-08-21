using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Sharporum.Domain.Entities;
using Sharporum.Domain.Infrastructure;
using Sharporum.Domain.Models;

namespace Sharporum.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<TResult>> ListUserFollowersAsync<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Followers
                .Include(x => x.FollowerUser)
                .Where(x => x.UserToFollowId == userId)
                .ProjectTo<TResult>(configurationProvider)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListUserFollowingAsync<TResult>(string userId,
            IConfigurationProvider configurationProvider) where TResult : class
        {
            return await _context.Followers
                .Include(x => x.UserToFollow)
                .Where(x => x.FollowerUserId == userId)
                .ProjectTo<TResult>(configurationProvider)
                .ToListAsync();
        }

        public async Task<bool> IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId)
        {
            return await _context.Followers
                .Where(x => x.UserToFollowId == userToFollowId)
                .FirstOrDefaultAsync(x => x.FollowerUserId == authenticatedUserId) != null;
        }

        public async Task FollowUserAsync(Follower follower)
        {
            await _context.Followers.AddAsync(follower);

            await _context.SaveChangesAsync();
        }

        public async Task UnfollowUserAsync(string userToFollowId, string followerUserId)
        {
            Follower f = await _context.Followers
                .Where(x => x.UserToFollowId == userToFollowId)
                .FirstOrDefaultAsync(x => x.FollowerUserId == followerUserId);

            _context.Followers.Remove(f);

            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<string>> ListUserFollowingsAsync(string userId)
        {
            return await _context.Followers
                .Where(x => x.FollowerUserId == userId)
                .Select(x => x.UserToFollowId)
                .ToListAsync();
        }

        public async Task<double> GetUserRank<TVoteEntity>(string userId) where TVoteEntity : BaseVoteEntity
        {
            return await _context.Set<TVoteEntity>()
                .Where(x => x.UserId == userId)
                .GroupBy(x => x.UserId, (key, vote) => vote.Sum(x => x.Direction))
                .FirstOrDefaultAsync();
        }

        public async Task<IReadOnlyList<Ranks>> ListRanks<TVoteEntity>() where TVoteEntity : BaseVoteEntity
        {
            List<Ranks> userEntityVotes = _context.Set<TVoteEntity>()
                .GroupBy(x => x.UserId, (key, vote) =>
                    new Ranks
                    {
                        UserId = key,
                        Rank = vote.Sum(x => x.Direction),
                    })
                .ToList();

            return userEntityVotes;
        }
    }
}