namespace Sharporum.Core.Dtos.Post
{
    public class CreatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string CommunityId { get; set; }
    }
}