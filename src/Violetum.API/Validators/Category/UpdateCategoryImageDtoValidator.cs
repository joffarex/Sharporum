using FluentValidation;
using Violetum.ApplicationCore.Dtos.Category;

namespace Violetum.API.Validators.Category
{
    public class UpdateCategoryImageDtoValidator : AbstractValidator<UpdateCategoryImageDto>
    {
        public UpdateCategoryImageDtoValidator()
        {
            RuleFor(x => x.Image).NotEmpty()
                .Matches("@(https?:)?//?[^'\"<>]+?\\.(jpg|jpeg|gif|png)@");
        }
    }
}