using MediatR;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Queries.Post
{
    public class GetPostsQuery : IRequest<FilteredDataViewModel<PostViewModel>>
    {
        public GetPostsQuery(PostSearchParams searchParams)
        {
            SearchParams = searchParams;
        }

        public PostSearchParams SearchParams { get; set; }
    }
}