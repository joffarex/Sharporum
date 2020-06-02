using FluentValidation;
using Violetum.ApplicationCore.Dtos.Post;

namespace Violetum.API.Validators.Post
{
    [FluentValidator]
    public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
    {
        public CreatePostDtoValidator()
        {
            RuleFor(x => x.AuthorId).NotNull();
            RuleFor(x => x.CategoryId).NotNull();
            RuleFor(x => x.Title).NotNull().Length(10, 255);
            RuleFor(x => x.Content).NotNull();
        }
    }
}