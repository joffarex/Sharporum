using MediatR;
using Sharporum.Core.Dtos.Post;

namespace Sharporum.Core.Commands.Post
{
    public class CreatePostCommand : IRequest<string>
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