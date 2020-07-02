using MediatR;
using Violetum.ApplicationCore.Dtos.Post;

namespace Violetum.ApplicationCore.Commands.Post
{
    public class VotePostCommand : IRequest
    {
        public VotePostCommand(string userId, string postId, PostVoteDto postVoteDto)
        {
            UserId = userId;
            PostId = postId;
            PostVoteDto = postVoteDto;
        }

        public string UserId { get; set; }
        public string PostId { get; set; }
        public PostVoteDto PostVoteDto { get; set; }
    }
}