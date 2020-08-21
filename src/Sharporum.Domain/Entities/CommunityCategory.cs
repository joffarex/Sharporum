namespace Sharporum.Domain.Entities
{
    public class CommunityCategory : BaseEntity
    {
        public string CommunityId { get; set; }
        public Community Community { get; set; }

        public string CategoryId { get; set; }
        public Category Category { get; set; }
    }
}