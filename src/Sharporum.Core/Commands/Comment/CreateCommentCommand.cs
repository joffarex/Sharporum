using MediatR;
using Sharporum.Core.Dtos.Comment;

namespace Sharporum.Core.Commands.Comment
{
    public class CreateCommentCommand : IRequest<string>
    {
        public CreateCommentCommand(string userId, CreateCommentDto createCommentDto)
        {
            UserId = userId;
            CreateCommentDto = createCommentDto;
        }

        public string UserId { get; set; }
        public CreateCommentDto CreateCommentDto { get; set; }
    }
}