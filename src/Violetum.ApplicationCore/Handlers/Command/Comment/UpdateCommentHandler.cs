using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Comment;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.ApplicationCore.Handlers.Command.Comment
{
    public class UpdateCommentHandler : IRequestHandler<UpdateCommentCommand, CommentResponse>
    {
        private readonly ICommentService _commentService;

        public UpdateCommentHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<CommentResponse> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
        {
            CommentViewModel commentViewModel =
                await _commentService.UpdateCommentAsync(request.Comment, request.UpdateCommentDto);

            return new CommentResponse {Comment = commentViewModel};
        }
    }
}