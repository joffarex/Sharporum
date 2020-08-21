using AutoMapper;
using Sharporum.Core.ViewModels.User;

namespace Sharporum.API.Profiles
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