using AutoMapper;
using Violetum.ApplicationCore.Dtos.Profile;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.Web.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<ProfileViewModel, UpdateProfileDto>();
        }
    }
}