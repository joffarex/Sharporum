using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Community;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Handlers.Query.Community
{
    public class GetCommunitiesHandler : IRequestHandler<GetCommunitiesQuery, GetManyResponse<CommunityViewModel>>
    {
        private readonly ICommunityService _communityService;

        public GetCommunitiesHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<GetManyResponse<CommunityViewModel>> Handle(GetCommunitiesQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<CommunityViewModel> communities =
                await _communityService.GetCommunitiesAsync(request.SearchParams);
            int communitiesCount = await _communityService.GetCategoriesCountAsync(request.SearchParams);

            return new GetManyResponse<CommunityViewModel>
            {
                Data = communities,
                Count = communitiesCount,
                Params = new Params
                    {Limit = request.SearchParams.Limit, CurrentPage = request.SearchParams.CurrentPage},
            };
        }
    }
}