namespace Violetum.ApplicationCore.Dtos.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string AuthorId { get; set; }
    }
}