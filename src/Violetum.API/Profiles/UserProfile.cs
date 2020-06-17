using AutoMapper;
using Violetum.ApplicationCore.Dtos.User;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserBaseViewModel>();
            CreateMap<User, UserViewModel>();
        }
    }
}