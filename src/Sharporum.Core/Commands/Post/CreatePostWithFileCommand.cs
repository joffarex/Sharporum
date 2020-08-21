using MediatR;
using Sharporum.Core.Dtos.Post;

namespace Sharporum.Core.Commands.Post
{
    public class CreatePostWithFileCommand : IRequest<string>
    {
        public CreatePostWithFileCommand(string userId, CreatePostWithFileDto createPostWithFileDto)
        {
            UserId = userId;
            CreatePostWithFileDto = createPostWithFileDto;
        }

        public string UserId { get; set; }
        public CreatePostWithFileDto CreatePostWithFileDto { get; set; }
    }
}