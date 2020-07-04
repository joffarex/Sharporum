using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.Community
{
    public class CreateCommunityHandler : IRequestHandler<CreateCommunityCommand, string>
    {
        private readonly ICommunityService _communityService;

        public CreateCommunityHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<string> Handle(CreateCommunityCommand request, CancellationToken cancellationToken)
        {
            return await _communityService.CreateCommunityAsync(request.UserId, request.CreateCommunityDto);
        }
    }
}