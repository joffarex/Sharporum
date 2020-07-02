using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Queries.Comment;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.ApplicationCore.Handlers.Query.Comment
{
    public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, GetManyResponse<CommentViewModel>>
    {
        private readonly ICommentService _commentService;

        public GetCommentsHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<GetManyResponse<CommentViewModel>> Handle(GetCommentsQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<CommentViewModel> comments = await _commentService.GetCommentsAsync(request.SearchParams);
            int commentsCount = await _commentService.GetCommentsCountAsync(request.SearchParams);

            return new GetManyResponse<CommentViewModel>
            {
                Data = comments,
                Count = commentsCount,
                Params = new Params
                    {Limit = request.SearchParams.Limit, CurrentPage = request.SearchParams.CurrentPage},
            };
        }
    }
}