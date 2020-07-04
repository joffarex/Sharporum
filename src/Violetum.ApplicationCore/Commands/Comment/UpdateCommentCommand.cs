using MediatR;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.ApplicationCore.Commands.Comment
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