using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.ApplicationCore.Commands.Comment
{
    public class CreateCommentCommand : IRequest<CreatedResponse>
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