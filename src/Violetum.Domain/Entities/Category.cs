using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Violetum.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }

        [Column(TypeName = "ntext")] public string Description { get; set; }

        public string Image { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public ICollection<Post> Posts { get; set; }
    }
}