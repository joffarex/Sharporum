using FluentValidation;
using Sharporum.Core.Dtos.Comment;

namespace Sharporum.API.Validators.Comment
{
    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}