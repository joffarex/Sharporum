using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.ViewModels.User;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserViewModel> GetUser(string id);
        Task<UserViewModel> UpdateUser(string userId, UpdateUserDto updateUserDto);
        Task<UserViewModel> UpdateUserImage(string userId, UpdateUserImageDto updateUserImageDto);
    }
}