using FluentValidation;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.API.Validators.Comment
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