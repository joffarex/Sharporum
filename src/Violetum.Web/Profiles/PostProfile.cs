using AutoMapper;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Entities;

namespace Violetum.Web.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostViewModel>();
            CreateMap<CreatePostDto, Post>();
            CreateMap<Post, CommentPostViewModel>();
        }
    }
}