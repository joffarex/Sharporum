using MediatR;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Queries.Comment
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