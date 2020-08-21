using System.Linq;
using AutoMapper;
using Sharporum.Core.ViewModels.Comment;
using Sharporum.Core.ViewModels.User;
using Sharporum.Domain.Entities;

namespace Sharporum.Core.Helpers
{
    public static class CommentHelpers
    {
        public static bool UserOwnsComment(string userId, string commentAuthorId)
        {
            return userId == commentAuthorId;
        }

        public static IConfigurationProvider GetCommentMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Comment, CommentViewModel>()
                    .ForMember(
                        c => c.VoteCount,
                        opt => opt.MapFrom(
                            x => x.CommentVotes.Sum(y => y.Direction)
                        )
                    );

                cfg.CreateMap<User, UserBaseViewModel>();
                cfg.CreateMap<Post, CommentPostViewModel>();
            });
        }
    }
}