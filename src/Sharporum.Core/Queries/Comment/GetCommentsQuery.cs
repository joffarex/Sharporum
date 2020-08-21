using MediatR;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Comment;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Queries.Comment
{
    public class GetCommentsQuery : IRequest<FilteredDataViewModel<CommentViewModel>>
    {
        public GetCommentsQuery(CommentSearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public CommentSearchParams SearchParams { get; set; }
    }
}