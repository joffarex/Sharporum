using AutoMapper;
using Sharporum.Core.ViewModels.Follower;
using Sharporum.Core.ViewModels.User;
using Sharporum.Domain.Entities;

namespace Sharporum.Core.Helpers
{
    public class UserHelpers
    {
        public static IConfigurationProvider GetFollowerMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Follower, FollowerViewModel>();
                cfg.CreateMap<Follower, FollowingViewModel>();

                cfg.CreateMap<User, UserBaseViewModel>();
            });
        }
    }
}