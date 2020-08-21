using MediatR;
using Sharporum.Core.ViewModels.Comment;

namespace Sharporum.Core.Queries.Comment
{
    public class GetCommentQuery : IRequest<CommentViewModel>
    {
        public GetCommentQuery(string commentId)
        {
            CommentId = commentId;
        }

        public string CommentId { get; set; }
    }
}