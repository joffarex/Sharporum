using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Queries.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.ApplicationCore.Handlers.Query.Comment
{
    public class GetCommentHandler : IRequestHandler<GetCommentQuery, CommentResponse>
    {
        private readonly ICommentService _commentService;

        public GetCommentHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public Task<CommentResponse> Handle(GetCommentQuery request, CancellationToken cancellationToken)
        {
            CommentViewModel comment = _commentService.GetComment(request.CommentId);
            return Task.FromResult(new CommentResponse {Comment = comment});
        }
    }
}