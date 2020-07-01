using MediatR;

namespace Violetum.ApplicationCore.Commands.Comment
{
    public class DeleteCommentCommand : IRequest
    {
        public DeleteCommentCommand(Domain.Entities.Comment comment)
        {
            Comment = comment;
        }

        public Domain.Entities.Comment Comment { get; set; }
    }
}