using MediatR;
using Sharporum.Core.Dtos.Comment;

namespace Sharporum.Core.Commands.Comment
{
    public class VoteCommentCommand : IRequest
    {
        public VoteCommentCommand(string userId, string commentId, CommentVoteDto commentVoteDto)
        {
            UserId = userId;
            CommentId = commentId;
            CommentVoteDto = commentVoteDto;
        }

        public string UserId { get; set; }
        public string CommentId { get; set; }
        public CommentVoteDto CommentVoteDto { get; set; }
    }
}