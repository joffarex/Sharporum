namespace Violetum.ApplicationCore.ViewModels
{
    public class CategoryViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public AuthorViewModel Author { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}