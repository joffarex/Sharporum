namespace Violetum.Domain.Entities
{
    public class CommentVote : BaseVoteEntity
    {
        public string CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}