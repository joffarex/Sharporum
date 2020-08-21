using System.Linq;
using AutoMapper;
using Sharporum.Core.ViewModels.Post;
using Sharporum.Core.ViewModels.User;
using Sharporum.Domain.Entities;

namespace Sharporum.Core.Helpers
{
    public class PostHelpers
    {
        public static bool UserOwnsPost(string userId, string postAuthorId)
        {
            return userId == postAuthorId;
        }

        public static IConfigurationProvider GetPostMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Post, PostViewModel>()
                    .ForMember(
                        p => p.VoteCount,
                        opt => opt.MapFrom(
                            x => x.PostVotes.Sum(y => y.Direction)
                        )
                    );

                cfg.CreateMap<User, UserBaseViewModel>();
                cfg.CreateMap<Community, PostCommunityViewModel>();
            });
        }

        public static bool IsContentFile(string content)
        {
            string[] dataUri = content.Split(",");

            string[] contentParts = dataUri[0].Split("/");

            return "data".Equals(contentParts[0].Split(":")[0]) && "base64".Equals(contentParts[1].Split(";")[1]);
        }
    }
}