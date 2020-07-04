using MediatR;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.ApplicationCore.Commands.Post
{
    public class UpdatePostCommand : IRequest<PostViewModel>
    {
        public UpdatePostCommand(Domain.Entities.Post post, UpdatePostDto updatePostDto)
        {
            Post = post;
            UpdatePostDto = updatePostDto;
        }

        public Domain.Entities.Post Post { get; set; }
        public UpdatePostDto UpdatePostDto { get; set; }
    }
}