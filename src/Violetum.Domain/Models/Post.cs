using System;

namespace Violetum.Domain.Models
{
    public class Post : BaseEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        // public Guid AuthorId { get; set; }
        // author
        // public Guid CategoryId { get; set; }
        // category
    }
}