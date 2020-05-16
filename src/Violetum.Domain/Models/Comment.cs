using System.ComponentModel.DataAnnotations.Schema;

namespace Violetum.Domain.Models
{
    public class Comment : BaseEntity
    {
        [Column(TypeName = "ntext")] public string Content { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public string PostId { get; set; }
        public Post Post { get; set; }
    }
}