using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Violetum.ApplicationCore.Commands.Community;
using Violetum.ApplicationCore.Interfaces.Services;

namespace Violetum.ApplicationCore.Handlers.Command.Community
{
    public class DeleteCommunityHandler : IRequestHandler<DeleteCommunityCommand>
    {
        private readonly IBlobService _blobService;
        private readonly ICommunityService _communityService;

        public DeleteCommunityHandler(ICommunityService communityService, IBlobService blobService)
        {
            _communityService = communityService;
            _blobService = blobService;
        }

        public async Task<Unit> Handle(DeleteCommunityCommand request, CancellationToken cancellationToken)
        {
            if (!request.Community.Image.Equals($"{nameof(Community)}/no-image.jpg"))
            {
                await _blobService.DeleteBlobAsync(request.Community.Image);
            }

            await _communityService.DeleteCommunityAsync(request.Community);
            return Unit.Value;
        }
    }
}