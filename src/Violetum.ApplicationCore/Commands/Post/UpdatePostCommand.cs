using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Post;

namespace Violetum.ApplicationCore.Commands.Post
{
    public class UpdatePostCommand : IRequest<PostResponse>
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