using Sharporum.Core.ViewModels.User;

namespace Sharporum.Core.ViewModels.Comment
{
    public class CommentViewModel
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string ParentId { get; set; }
        public UserBaseViewModel Author { get; set; }
        public CommentPostViewModel Post { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public int VoteCount { get; set; }
    }
}