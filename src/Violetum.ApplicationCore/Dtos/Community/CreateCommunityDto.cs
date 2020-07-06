using System.Collections.Generic;

namespace Violetum.ApplicationCore.Dtos.Community
{
    public class CreateCommunityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> CategoryIds { get; set; }
    }
}