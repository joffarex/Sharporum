using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Violetum.Domain.Entities
{
    public class Post : BaseEntity
    {
        public string Title { get; set; }

        [Column(TypeName = "ntext")] public string Content { get; set; }
        public string ContentType { get; set; }

        public string AuthorId { get; set; }
        public User Author { get; set; }

        public string CommunityId { get; set; }
        public Community Community { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostVote> PostVotes { get; set; }
    }
}