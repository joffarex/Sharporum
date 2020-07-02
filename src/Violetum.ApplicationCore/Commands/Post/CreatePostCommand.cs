using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Post;

namespace Violetum.ApplicationCore.Commands.Post
{
    public class CreatePostCommand : IRequest<CreatedResponse>
    {
        public CreatePostCommand(string userId, CreatePostDto createPostDto)
        {
            UserId = userId;
            CreatePostDto = createPostDto;
        }

        public string UserId { get; set; }
        public CreatePostDto CreatePostDto { get; set; }
    }
}