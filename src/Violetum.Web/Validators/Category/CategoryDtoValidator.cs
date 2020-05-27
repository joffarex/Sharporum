using FluentValidation;
using Violetum.ApplicationCore.Dtos.Category;

namespace Violetum.Web.Validators.Category
{
    public class CategoryDtoValidator : AbstractValidator<CategoryDto>
    {
        public CategoryDtoValidator()
        {
            RuleFor(x => x.AuthorId).NotNull();
            RuleFor(x => x.Name).NotNull().Length(10, 255);
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.Image).NotNull();
        }
    }
}