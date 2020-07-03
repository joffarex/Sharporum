using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Community;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Handlers.Query.Community
{
    public class GetCommunityHandler : IRequestHandler<GetCommunityQuery, CommunityResponse>
    {
        private readonly ICommunityService _communityService;

        public GetCommunityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public Task<CommunityResponse> Handle(GetCommunityQuery request, CancellationToken cancellationToken)
        {
            CommunityViewModel community = _communityService.GetCommunityById(request.CommunityId);
            return Task.FromResult(new CommunityResponse {Community = community});
        }
    }
}