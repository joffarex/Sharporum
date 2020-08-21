using MediatR;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Post;
using Sharporum.Domain.Models.SearchParams;

namespace Sharporum.Core.Queries.Post
{
    public class GetNewsFeedQuery : IRequest<FilteredDataViewModel<PostViewModel>>
    {
        public GetNewsFeedQuery(string userId, PostSearchParams searchParams)
        {
            UserId = userId;
            SearchParams = searchParams;
        }

        public string UserId { get; set; }
        public PostSearchParams SearchParams { get; set; }
    }
}