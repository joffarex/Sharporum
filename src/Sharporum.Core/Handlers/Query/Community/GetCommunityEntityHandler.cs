using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Interfaces;
using Sharporum.Core.Queries.Community;

namespace Sharporum.Core.Handlers.Query.Community
{
    public class GetCommunityEntityHandler : IRequestHandler<GetCommunityEntityQuery, Domain.Entities.Community>
    {
        private readonly ICommunityService _communityService;

        public GetCommunityEntityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<Domain.Entities.Community> Handle(GetCommunityEntityQuery request,
            CancellationToken cancellationToken)
        {
            return await _communityService.GetCommunityEntity(request.CommunityId);
        }
    }
}