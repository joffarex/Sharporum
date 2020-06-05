using FluentValidation;
using Violetum.ApplicationCore.Dtos.Post;

namespace Violetum.API.Validators.Post
{
    [FluentValidator]
    public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
    {
        public CreatePostDtoValidator()
        {
            RuleFor(x => x.AuthorId).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.Title).NotNull().Length(10, 255).Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}