using MediatR;

namespace Violetum.ApplicationCore.Commands.Community
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