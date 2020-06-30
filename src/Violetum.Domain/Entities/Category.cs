using System.Collections.Generic;

namespace Violetum.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public IEnumerable<Community> Communities { get; set; }
    }
}