using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Community;
using Sharporum.Core.ViewModels.Community;

namespace Sharporum.Core.Handlers.Query.Community
{
    public class GetCommunityHandler : IRequestHandler<GetCommunityQuery, CommunityViewModel>
    {
        private readonly ICommunityService _communityService;

        public GetCommunityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<CommunityViewModel> Handle(GetCommunityQuery request, CancellationToken cancellationToken)
        {
            return await _communityService.GetCommunityByIdAsync(request.CommunityId);
        }
    }
}