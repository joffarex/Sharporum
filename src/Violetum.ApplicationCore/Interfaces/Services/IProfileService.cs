using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.ViewModels.User;

namespace Violetum.ApplicationCore.Interfaces.Services
{
    public interface IProfileService
    {
        Task<ProfileViewModel> GetProfile(string id);
        Task<ProfileViewModel> UpdateProfile(string userId, UpdateProfileDto updateProfileDto);
    }
}