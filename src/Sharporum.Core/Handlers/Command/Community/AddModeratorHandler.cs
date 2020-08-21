using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sharporum.Core.Commands.Community;
using Sharporum.Core.Interfaces;

namespace Sharporum.Core.Handlers.Command.Community
{
    public class AddModeratorHandler : IRequestHandler<AddModeratorCommand>
    {
        private readonly ICommunityService _communityService;

        public AddModeratorHandler(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task<Unit> Handle(AddModeratorCommand request, CancellationToken cancellationToken)
        {
            await _communityService.AddModeratorAsync(request.Community, request.AddModeratorDto);

            return Unit.Value;
        }
    }
}