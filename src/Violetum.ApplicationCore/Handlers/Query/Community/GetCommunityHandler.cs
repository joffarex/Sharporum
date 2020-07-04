using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Interfaces;
using Violetum.ApplicationCore.Queries.Community;
using Violetum.ApplicationCore.ViewModels.Community;

namespace Violetum.ApplicationCore.Handlers.Query.Community
{
    public class GetCommunityHandler : IRequestHandler<GetCommunityQuery, CommunityViewModel>
    {
        private readonly ICommunityService _communityService;

        public GetCommunityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public Task<CommunityViewModel> Handle(GetCommunityQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_communityService.GetCommunityById(request.CommunityId));
        }
    }
}