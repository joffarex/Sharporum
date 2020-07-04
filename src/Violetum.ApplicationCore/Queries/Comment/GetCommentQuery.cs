using MediatR;
using Violetum.ApplicationCore.ViewModels.Comment;

namespace Violetum.ApplicationCore.Queries.Comment
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