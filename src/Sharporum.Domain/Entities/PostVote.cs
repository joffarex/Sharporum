namespace Sharporum.Domain.Entities
{
    public class PostVote : BaseVoteEntity
    {
        public string PostId { get; set; }
        public Post Post { get; set; }
    }
}