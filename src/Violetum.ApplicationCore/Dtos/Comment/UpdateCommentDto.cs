namespace Violetum.ApplicationCore.Dtos.Comment
{
    public class UpdateCommentDto
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string PostId { get; set; }
    }
}