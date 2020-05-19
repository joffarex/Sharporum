using System.Threading.Tasks;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.ViewModels;

namespace Violetum.ApplicationCore.Interfaces
{
    public interface IProfileService
    {
        Task<ProfileViewModel> GetProfile(string id);
        Task<ProfileViewModel> UpdateProfile(string userId, UpdateProfileDto updateProfileDto);
    }
}