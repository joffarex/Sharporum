using System.Threading.Tasks;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IFollowerService
    {
        Task<UserFollowersViewModel> GetUserFollowers(string userId);
        Task<UserFollowingViewModel> GetUserFollowing(string userId);

        bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId);

        Task FollowUser(string userId, string userToFollowId);
        Task UnfollowUser(string userId, string userToUnfollowId);
    }
}