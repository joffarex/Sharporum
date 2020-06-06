using AutoMapper;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Entities;

namespace Violetum.API.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostViewModel>();
            CreateMap<PostViewModel, Post>();
            CreateMap<CreatePostDto, Post>();
            CreateMap<Post, CommentPostViewModel>();
        }
    }
}