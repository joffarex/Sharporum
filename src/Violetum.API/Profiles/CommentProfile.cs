using System.Linq;
using AutoMapper;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Entities;

namespace Violetum.API.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentViewModel>()
                .ForMember(
                    c => c.VoteCount,
                    opt => opt.MapFrom(
                        x => x.CommentVotes.Sum(y => y.Direction)
                    )
                );
            CreateMap<CommentViewModel, Comment>();
            CreateMap<CreateCommentDto, Comment>();
        }
    }
}