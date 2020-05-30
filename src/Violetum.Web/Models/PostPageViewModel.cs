using System.Collections.Generic;
using Violetum.ApplicationCore.Dtos.Comment;
using Violetum.ApplicationCore.Dtos.Post;
using Violetum.ApplicationCore.ViewModels.Comment;
using Violetum.ApplicationCore.ViewModels.Post;

namespace Violetum.Web.Models
{
    public class PostPageViewModel
    {
        public PostViewModel Post { get; set; }
        public IEnumerable<CommentViewModel> Comments { get; set; }
        public CreateCommentDto CreateCommentDto { get; set; }
        public PostVoteDto PostVoteDto { get; set; }
        public CommentVoteDto CommentVoteDto { get; set; }
    }
}