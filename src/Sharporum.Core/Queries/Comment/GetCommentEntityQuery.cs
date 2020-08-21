using MediatR;

namespace Sharporum.Core.Queries.Comment
{
    public class GetCommentEntityQuery : IRequest<Domain.Entities.Comment>
    {
        public GetCommentEntityQuery(string commentId)
        {
            CommentId = commentId;
        }

        public string CommentId { get; set; }
    }
}