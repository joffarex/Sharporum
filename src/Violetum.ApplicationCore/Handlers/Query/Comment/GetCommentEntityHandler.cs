using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Queries.Comment;

namespace Violetum.ApplicationCore.Handlers.Query.Comment
{
    public class GetCommentEntityHandler : IRequestHandler<GetCommentEntityQuery, Domain.Entities.Comment>
    {
        private readonly ICommentService _commentService;

        public GetCommentEntityHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public Task<Domain.Entities.Comment> Handle(GetCommentEntityQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_commentService.GetCommentEntity(request.CommentId));
        }
    }
}