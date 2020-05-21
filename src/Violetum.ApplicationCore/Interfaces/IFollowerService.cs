using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Follower;
using Violetum.ApplicationCore.ViewModels;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface IFollowerService
    {
        Task<UserFollowersViewModel> GetUserFollowers(string userId);
        Task<UserFollowingViewModel> GetUserFollowing(string userId);

        bool IsAuthenticatedUserFollower(string userToFollowId, string authenticatedUserId);

        Task FollowUser(FollowerDto followerDto);
        Task UnfollowUser(FollowerDto followerDto);
    }
}