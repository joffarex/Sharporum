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
    public class GetPostsHandler : IRequestHandler<GetNewsFeedQuery, GetManyResponse<PostViewModel>>
    {
        private readonly IPostService _postService;

        public GetPostsHandler(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<GetManyResponse<PostViewModel>> Handle(GetNewsFeedQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<PostViewModel> posts = _postService.GetNewsFeedPosts(request.UserId, request.SearchParams);
            int postsCount = _postService.GetPostsCountInNewsFeed(request.UserId, request.SearchParams);

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