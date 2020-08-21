using FluentValidation;
using Sharporum.Core.Dtos.Post;

namespace Sharporum.API.Validators.Post
{
    public class CreatePostWithFileDtoValidator : AbstractValidator<CreatePostWithFileDto>
    {
        public CreatePostWithFileDtoValidator()
        {
            RuleFor(x => x.CommunityId).NotEmpty();
            RuleFor(x => x.Title).NotNull().Length(10, 255).Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Content).NotEmpty();
            RuleFor(x => x.ContentType).NotEmpty();
        }
    }
}