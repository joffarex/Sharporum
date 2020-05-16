using System.ComponentModel.DataAnnotations.Schema;

namespace Violetum.Domain.Models
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        [Column(TypeName = "ntext")] public string Description { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }
    }
}