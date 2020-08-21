using MediatR;

namespace Sharporum.Core.Commands.Community
{
    public class DeleteCommunityCommand : IRequest
    {
        public DeleteCommunityCommand(Domain.Entities.Community community)
        {
            Community = community;
        }

        public Domain.Entities.Community Community { get; }
    }
}