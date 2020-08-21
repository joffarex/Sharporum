using MediatR;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Post;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Queries.Post
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