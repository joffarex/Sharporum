using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Queries.Post
{
    public class GetPostsQuery : IRequest<GetManyResponse<PostViewModel>>
    {
        public GetPostsQuery(PostSearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public PostSearchParams SearchParams { get; set; }
    }
}