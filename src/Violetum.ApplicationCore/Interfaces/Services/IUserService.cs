using System.Collections.Generic;
using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Models;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserViewModel> GetUser(string userId);
        Task<UserViewModel> UpdateUser(string userId, UpdateUserDto updateUserDto);
        Task<UserViewModel> UpdateUserImage(string userId, UpdateUserImageDto updateUserImageDto);

        IEnumerable<UserRank> GetUserRanks(string userId);

        IEnumerable<Ranks> GetPostRanks();
        IEnumerable<Ranks> GetCommentRanks();
    }
}