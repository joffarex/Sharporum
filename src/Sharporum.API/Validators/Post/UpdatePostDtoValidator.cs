using FluentValidation;
using Sharporum.Core.Dtos.Post;

namespace Sharporum.API.Validators.Post
{
    public class UpdatePostDtoValidator : AbstractValidator<UpdatePostDto>
    {
        public UpdatePostDtoValidator()
        {
            RuleFor(x => x.Title).NotNull().Length(10, 255).Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}