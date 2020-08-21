using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Comment;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Comment;

namespace Sharporum.Core.Handlers.Query.Comment
{
    public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, FilteredDataViewModel<CommentViewModel>>
    {
        private readonly ICommentService _commentService;

        public GetCommentsHandler(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public async Task<FilteredDataViewModel<CommentViewModel>> Handle(GetCommentsQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<CommentViewModel> comments = await _commentService.GetCommentsAsync(request.SearchParams);
            int commentsCount = await _commentService.GetCommentsCountAsync(request.SearchParams);

            return new FilteredDataViewModel<CommentViewModel>
            {
                Data = comments,
                Count = commentsCount,
            };
        }
    }
}