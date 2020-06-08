namespace Violetum.ApplicationCore.Dtos.Comment
{
    public class CreateCommentDto
    {
        public string Content { get; set; }
        public string ParentId { get; set; }
        public string PostId { get; set; }
    }
}