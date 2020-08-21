namespace Sharporum.Domain.Entities
{
    public class Follower : BaseEntity
    {
        public string UserToFollowId { get; set; }
        public User UserToFollow { get; set; }
        public string FollowerUserId { get; set; }
        public User FollowerUser { get; set; }
    }
}