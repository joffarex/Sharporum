using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Contracts.V1.Responses;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.Community
{
    public class CreateCommunityHandler : IRequestHandler<CreateCommunityCommand, CreatedResponse>
    {
        private readonly ICommunityService _communityService;

        public CreateCommunityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<CreatedResponse> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
        {
            string communityId =
                await _communityService.CreateCommunityAsync(request.UserId, request.CreateCommunityDto);
            return new CreatedResponse {Id = communityId};
        }
    }
}