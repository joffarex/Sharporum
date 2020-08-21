using AutoMapper;
using Sharporum.Core.Dtos.Post;
using Sharporum.Core.ViewModels.Comment;
using Sharporum.Core.ViewModels.Post;

namespace Sharporum.API.Profiles
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
            CreateMap<CreatePostDto, Post>();
            CreateMap<Post, CommentPostViewModel>();
        }
    }
}