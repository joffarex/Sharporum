using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Post;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.ApplicationCore.Handlers.Query.Post
{
    public class GetNewsFeedHandler : IRequestHandler<GetNewsFeedQuery, FilteredDataViewModel<PostViewModel>>
    {
        private readonly IPostService _postService;

        public GetNewsFeedHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<FilteredDataViewModel<PostViewModel>> Handle(GetNewsFeedQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<PostViewModel> posts = _postService.GetNewsFeedPosts(request.UserId, request.SearchParams);
            int postsCount = _postService.GetPostsCountInNewsFeed(request.UserId, request.SearchParams);

            return new FilteredDataViewModel<PostViewModel>
            {
                Data = posts,
                Count = postsCount,
            };
        }
    }
}