using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Interfaces;

namespace Violetum.ApplicationCore.Handlers.Command.Community
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