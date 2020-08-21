using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Comment;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Comment
{
    public class DeleteCommentHandler : IRequestHandler<DeleteCommentCommand>
    {
        private readonly ICommentService _commentService;

        public DeleteCommentHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<Unit> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            await _commentService.DeleteCommentAsync(request.Comment);

            return Unit.Value;
        }
    }
}