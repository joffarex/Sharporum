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
    public class GetPostsHandler : IRequestHandler<GetPostsQuery, FilteredDataViewModel<PostViewModel>>
    {
        private readonly IPostService _postService;

        public GetPostsHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<FilteredDataViewModel<PostViewModel>> Handle(GetPostsQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<PostViewModel> posts = await _postService.GetPostsAsync(request.SearchParams);
            int postsCount = await _postService.GetPostsCountAsync(request.SearchParams);

            return new FilteredDataViewModel<PostViewModel>
            {
                Data = posts,
                Count = postsCount,
            };
        }
    }
}