using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Community;

namespace Violetum.ApplicationCore.Handlers.Query.Community
{
    public class GetCommunityEntityHandler : IRequestHandler<GetCommunityEntityQuery, Domain.Entities.Community>
    {
        private readonly ICommunityService _communityService;

        public GetCommunityEntityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public Task<Domain.Entities.Community> Handle(GetCommunityEntityQuery request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_communityService.GetCommunityEntity(request.CommunityId));
        }
    }
}