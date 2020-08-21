using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Comment;
using Sharporum.Core.ViewModels.Comment;

namespace Sharporum.Core.Handlers.Query.Comment
{
    public class GetCommentHandler : IRequestHandler<GetCommentQuery, CommentViewModel>
    {
        private readonly ICommentService _commentService;

        public GetCommentHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<CommentViewModel> Handle(GetCommentQuery request, CancellationToken cancellationToken)
        {
            return await _commentService.GetCommentByIdAsync(request.CommentId);
        }
    }
}