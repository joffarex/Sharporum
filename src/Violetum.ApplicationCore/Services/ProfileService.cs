using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class ProfileService : IProfileService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserValidators _userValidators;

        public ProfileService(UserManager<User> userManager, IMapper mapper, IUserValidators userValidators)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userValidators = userValidators;
        }

        public async Task<ProfileViewModel> GetProfile(string userId)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);

            return _mapper.Map<ProfileViewModel>(user);
        }

        public async Task<ProfileViewModel> UpdateProfile(string userId,
            UpdateProfileDto updateProfileDto)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);
            user.Email = updateProfileDto.Email;
            user.UserName = updateProfileDto.UserName;
            user.FirstName = updateProfileDto.FirstName;
            user.LastName = updateProfileDto.LastName;
            user.Gender = updateProfileDto.Gender;
            user.BirthDate = updateProfileDto.Birthdate;

            await _userManager.UpdateAsync(user);

            return _mapper.Map<ProfileViewModel>(user);
        }
    }
}