namespace Violetum.ApplicationCore.Dtos.Comment
{
    public class CommentDto
    {
        public string Content { get; set; }
        public string ParentId { get; set; }
        public string AuthorId { get; set; }
        public string PostId { get; set; }
    }
}