using AutoMapper;
using Violetum.ApplicationCore.ViewModels.Community;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
{
    public static class CommunityHelpers
    {
        public static bool UserOwnsCommunity(string userId, string communityAuthorId)
        {
            return userId == communityAuthorId;
        }

        public static IConfigurationProvider GetCommunityMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Community, CommunityViewModel>();

                cfg.CreateMap<User, UserBaseViewModel>();
            });
        }
    }
}