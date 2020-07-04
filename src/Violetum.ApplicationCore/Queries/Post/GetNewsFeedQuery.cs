using MediatR;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Post;
using Violetum.Domain.Models.SearchParams;

namespace Violetum.ApplicationCore.Queries.Post
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