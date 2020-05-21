namespace Violetum.ApplicationCore.Dtos.Post
{
    public class PostVoteDto
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public int Direction { get; set; }
    }
}