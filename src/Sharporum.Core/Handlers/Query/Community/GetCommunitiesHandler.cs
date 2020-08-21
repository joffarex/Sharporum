using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Community;
using Sharporum.Core.ViewModels;
using Sharporum.Core.ViewModels.Community;

namespace Sharporum.Core.Handlers.Query.Community
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