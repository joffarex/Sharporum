using System.Linq;
using AutoMapper;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.ApplicationCore.ViewModels.User;
using Violetum.Domain.Entities;

namespace Violetum.ApplicationCore.Helpers
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