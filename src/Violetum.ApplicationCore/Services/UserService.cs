using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Violetum.ApplicationCore.Attributes;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Interfaces.Validators;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Services
{
    [Service]
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserValidators _userValidators;

        public UserService(UserManager<User> userManager, IMapper mapper, IUserValidators userValidators)
        {
            _userManager = userManager;
            _mapper = mapper;
            _userValidators = userValidators;
        }

        public async Task<UserViewModel> GetUser(string userId)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);

            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<UserViewModel> UpdateUser(string userId,
            UpdateUserDto updateUserDto)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);
            user.Email = updateUserDto.Email;
            user.UserName = updateUserDto.UserName;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Gender = updateUserDto.Gender;
            user.BirthDate = updateUserDto.Birthdate;

            await _userManager.UpdateAsync(user);

            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<UserViewModel> UpdateUserImage(string userId,
            UpdateUserImageDto updateUserImageDto)
        {
            User user = await _userValidators.GetUserByIdOrThrow(userId);
            user.Image = updateUserImageDto.Image;

            await _userManager.UpdateAsync(user);

            return _mapper.Map<UserViewModel>(user);
        }
    }
}