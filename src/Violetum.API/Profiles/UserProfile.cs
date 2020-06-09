using AutoMapper;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel, User>();
            CreateMap<ProfileViewModel, UpdateProfileDto>();
        }
    }
}