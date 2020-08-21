using System.ComponentModel.DataAnnotations;

namespace Sharporum.Domain.Entities
{
    public class BaseVoteEntity : BaseEntity
    {
        public string UserId { get; set; }
        public User User { get; set; }

        [Range(-1, 1, ErrorMessage = "Incorrect Comment Vote Direction - {0}")]
        public int Direction { get; set; }
    }
}