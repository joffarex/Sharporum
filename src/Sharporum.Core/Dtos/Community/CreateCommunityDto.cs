using System.Collections.Generic;

namespace Sharporum.Core.Dtos.Community
{
    public class CreateCommunityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> CategoryIds { get; set; }
    }
}