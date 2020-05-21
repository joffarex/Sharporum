namespace Violetum.ApplicationCore.Dtos.Post
{
    public class CommentVoteDto
    {
        public string CommentId { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public int Direction { get; set; }
    }
}