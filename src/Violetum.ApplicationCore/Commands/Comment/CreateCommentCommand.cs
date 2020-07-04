using MediatR;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.ApplicationCore.Commands.Comment
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