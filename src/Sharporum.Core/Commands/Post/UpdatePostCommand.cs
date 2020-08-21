using MediatR;
using Sharporum.Core.Dtos.Post;
using Sharporum.Core.ViewModels.Post;

namespace Sharporum.Core.Commands.Post
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