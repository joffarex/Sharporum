using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Comment;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.ApplicationCore.Handlers.Command.Comment
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