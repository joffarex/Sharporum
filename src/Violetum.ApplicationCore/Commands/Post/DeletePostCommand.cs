using MediatR;

namespace Violetum.ApplicationCore.Commands.Post
{
    public class DeletePostCommand : IRequest
    {
        public DeletePostCommand(Domain.Entities.Post post)
        {
            Post = post;
        }

        public Domain.Entities.Post Post { get; set; }
    }
}