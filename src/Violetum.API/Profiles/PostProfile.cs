using System.Linq;
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
            CreateMap<Post, PostViewModel>()
                .ForMember(
                    p => p.VoteCount,
                    opt => opt.MapFrom(
                        x => x.PostVotes.Sum(y => y.Direction)
                    )
                );
            CreateMap<PostViewModel, Post>();
            CreateMap<CreatePostDto, Post>();
            CreateMap<Post, CommentPostViewModel>();
        }
    }
}