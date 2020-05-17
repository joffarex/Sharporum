using AutoMapper;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels;
using Violetum.Domain.Entities;

namespace Violetum.Web.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostViewModel>();
            CreateMap<PostDto, Post>();
        }
    }
}