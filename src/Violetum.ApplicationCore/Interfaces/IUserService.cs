using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface IUserService
    {
        Task<UserViewModel> GetUserAsync(string userId);
        Task<UserViewModel> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<UserViewModel> UpdateUserImageAsync(string userId, UpdateUserImageDto updateUserImageDto);

        Task<IReadOnlyList<UserRank>> GetUserRanksAsync(string userId);

        Task<IReadOnlyList<Ranks>> GetPostRanksAsync();
        Task<IReadOnlyList<Ranks>> GetCommentRanksAsync();

        Task<UserFollowersViewModel> GetUserFollowersAsync(string userId);
        Task<UserFollowingViewModel> GetUserFollowingAsync(string userId);

        Task<bool> IsAuthenticatedUserFollowerAsync(string userToFollowId, string authenticatedUserId);

        Task FollowUserAsync(string userId, string userToFollowId);
        Task UnfollowUserAsync(string userId, string userToUnfollowId);
    }
}