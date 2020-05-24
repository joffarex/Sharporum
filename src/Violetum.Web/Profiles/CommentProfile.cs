using AutoMapper;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Entities;

namespace Violetum.Web.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentViewModel>();
            CreateMap<CommentDto, Comment>();
        }
    }
}