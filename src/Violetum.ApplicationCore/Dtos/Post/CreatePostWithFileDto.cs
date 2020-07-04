namespace Violetum.ApplicationCore.Dtos.Post
{
    public class CreatePostWithFileDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string ContentType { get; set; }
        public string CommunityId { get; set; }
    }
}