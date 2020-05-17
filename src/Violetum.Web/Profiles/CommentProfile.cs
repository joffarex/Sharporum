using AutoMapper;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;

namespace Violetum.Web.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentViewModel>();
        }
    }
}