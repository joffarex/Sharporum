using Violetum.ApplicationCore.ViewModels.User;

namespace Violetum.ApplicationCore.ViewModels.Post
{
    public class PostViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public UserViewModel Author { get; set; }
        public PostCategoryViewModel Category { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public int VoteCount { get; set; }
    }
}