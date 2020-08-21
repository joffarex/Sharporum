using MediatR;
using Sharporum.Core.Dtos.Comment;
using Sharporum.Core.ViewModels.Comment;

namespace Sharporum.Core.Commands.Comment
{
    public class UpdateCommentCommand : IRequest<CommentViewModel>
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