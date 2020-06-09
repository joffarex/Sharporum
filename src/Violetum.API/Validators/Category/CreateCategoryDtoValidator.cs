using FluentValidation;
using Violetum.ApplicationCore.Dtos.Category;

namespace Violetum.API.Validators.Category
{
    [FluentValidator]
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(10, 255).Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Image).NotEmpty();
        }
    }
}