using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Comment;
using Sharporum.Core.Interfaces;
using Sharporum.Core.ViewModels.Comment;

namespace Sharporum.Core.Handlers.Command.Comment
{
    public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, CommentViewModel>
    {
        private readonly ICommentService _commentService;

        public UpdateCommentHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<CommentViewModel> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            return await _commentService.UpdateCommentAsync(request.Comment, request.UpdateCommentDto);
        }
    }
}