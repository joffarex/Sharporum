using FluentValidation;
using Violetum.ApplicationCore.Dtos.Category;

namespace Violetum.API.Validators.Category
{
    [FluentValidator]
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Name).NotNull().Length(10, 255);
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.Image).NotNull();
        }
    }
}