using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Comment;
using Violetum.ApplicationCore.Interfaces.Services;

namespace Violetum.ApplicationCore.Handlers.Command.Comment
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