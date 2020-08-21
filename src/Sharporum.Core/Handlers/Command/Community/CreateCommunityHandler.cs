using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Community;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Community
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