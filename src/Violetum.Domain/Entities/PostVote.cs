using System.ComponentModel.DataAnnotations;

namespace Violetum.Domain.Entities
{
    public class PostVote : BaseEntity
    {
        public string PostId { get; set; }
        public Post Post { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        [Range(-1, 1, ErrorMessage = "Incorrect Post Vote Direction - {0}")]
        public int Direction { get; set; }
    }
}