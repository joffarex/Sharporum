using System.ComponentModel.DataAnnotations.Schema;

namespace Violetum.Domain.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }

        [Column(TypeName = "ntext")] public string Content { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }
    }
}