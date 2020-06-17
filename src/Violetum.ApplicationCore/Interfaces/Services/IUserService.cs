using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserViewModel> GetUserAsync(string userId);
        Task<UserViewModel> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<UserViewModel> UpdateUserImageAsync(string userId, UpdateUserImageDto updateUserImageDto);

        IEnumerable<UserRank> GetUserRanks(string userId);

        IEnumerable<Ranks> GetPostRanks();
        IEnumerable<Ranks> GetCommentRanks();
    }
}