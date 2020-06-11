using AutoMapper;
using Violetum.ApplicationCore.ViewModels.Follower;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
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