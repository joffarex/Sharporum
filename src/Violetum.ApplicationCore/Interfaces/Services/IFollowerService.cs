using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.ViewModels.Follower;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IFollowerService
    {
        Task<UserFollowersViewModel> GetUserFollowers(string userId);
        Task<UserFollowingViewModel> GetUserFollowing(string userId);

        bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId);

        Task FollowUser(FollowActionDto followActionDto);
        Task UnfollowUser(FollowActionDto followActionDto);
    }
}