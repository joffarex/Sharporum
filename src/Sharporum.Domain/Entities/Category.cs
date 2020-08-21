using System.Collections.Generic;

namespace Sharporum.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<CommunityCategory> CommunityCategories { get; set; }
    }
}