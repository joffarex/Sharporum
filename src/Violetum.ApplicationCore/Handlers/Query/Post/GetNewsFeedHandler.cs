using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces.Services;
using Violetum.ApplicationCore.Queries.Post;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.ApplicationCore.Handlers.Query.Post
{
    public class GetNewsFeedHandler : IRequestHandler<GetPostsQuery, GetManyResponse<PostViewModel>>
    {
        private readonly IPostService _postService;

        public GetNewsFeedHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<GetManyResponse<PostViewModel>> Handle(GetPostsQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<PostViewModel> posts = await _postService.GetPostsAsync(request.SearchParams);
            int postsCount = await _postService.GetPostsCountAsync(request.SearchParams);

            return new GetManyResponse<PostViewModel>
            {
                Data = posts,
                Count = postsCount,
                Params = new Params
                    {Limit = request.SearchParams.Limit, CurrentPage = request.SearchParams.CurrentPage},
            };
        }
    }
}