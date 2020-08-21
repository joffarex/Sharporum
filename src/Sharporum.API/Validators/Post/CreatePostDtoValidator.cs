using FluentValidation;
using Sharporum.Core.Dtos.Post;

namespace Sharporum.API.Validators.Post
{
    public class CreatePostDtoValidator : AbstractValidator<CreatePostDto>
    {
        public CreatePostDtoValidator()
        {
            RuleFor(x => x.CommunityId).NotEmpty();
            RuleFor(x => x.Title).NotNull().Length(10, 255).Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}