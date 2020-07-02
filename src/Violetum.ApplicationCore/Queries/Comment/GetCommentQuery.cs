using MediatR;
using Violetum.ApplicationCore.Contracts.V1.Responses;

namespace Violetum.ApplicationCore.Queries.Comment
{
    public class GetCommentQuery : IRequest<CommentResponse>
    {
        public GetCommentQuery(string commentId)
        {
            CommentId = commentId;
        }

        public string CommentId { get; set; }
    }
}