using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.ApplicationCore.Handlers.Query.Comment
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