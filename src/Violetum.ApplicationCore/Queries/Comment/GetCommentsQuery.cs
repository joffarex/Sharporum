using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Queries.Comment
{
    public class GetCommentsQuery : IRequest<GetManyResponse<CommentViewModel>>
    {
        public GetCommentsQuery(CommentSearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public CommentSearchParams SearchParams { get; set; }
    }
}