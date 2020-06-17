using System.Threading.Tasks;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IFollowerService
    {
        Task<UserFollowersViewModel> GetUserFollowersAsync(string userId);
        Task<UserFollowingViewModel> GetUserFollowingAsync(string userId);

        bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId);

        Task FollowUserAsync(string userId, string userToFollowId);
        Task UnfollowUserAsync(string userId, string userToUnfollowId);
    }
}