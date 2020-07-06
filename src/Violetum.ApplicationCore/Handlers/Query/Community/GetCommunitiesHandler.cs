using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Community;
using Violetum.ApplicationCore.ViewModels;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Handlers.Query.Community
{
    public class GetCommunitiesHandler : IRequestHandler<GetCommunitiesQuery, FilteredDataViewModel<CommunityViewModel>>
    {
        private readonly ICommunityService _communityService;

        public GetCommunitiesHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<FilteredDataViewModel<CommunityViewModel>> Handle(GetCommunitiesQuery request,
            CancellationToken cancellationToken)
        {
            IEnumerable<CommunityViewModel> communities =
                await _communityService.GetCommunitiesAsync(request.SearchParams);
            int communitiesCount = await _communityService.GetCommunitiesCountAsync(request.SearchParams);

            return new FilteredDataViewModel<CommunityViewModel>
            {
                Data = communities,
                Count = communitiesCount,
            };
        }
    }
}