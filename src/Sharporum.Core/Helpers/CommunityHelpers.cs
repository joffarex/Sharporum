using AutoMapper;
using Sharporum.Core.ViewModels.Community;
using Sharporum.Core.ViewModels.User;
using Sharporum.Domain.Entities;

namespace Sharporum.Core.Helpers
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