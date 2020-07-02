using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.ApplicationCore.Commands.Comment
{
    public class UpdateCommentCommand : IRequest<CommentResponse>
    {
        public UpdateCommentCommand(Domain.Entities.Comment comment, UpdateCommentDto updateCommentDto)
        {
            Comment = comment;
            UpdateCommentDto = updateCommentDto;
        }

        public Domain.Entities.Comment Comment { get; set; }
        public UpdateCommentDto UpdateCommentDto { get; set; }
    }
}