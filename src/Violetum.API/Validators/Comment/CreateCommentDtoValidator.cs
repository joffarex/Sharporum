using FluentValidation;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.API.Validators.Comment
{
    [FluentValidator]
    public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
    {
        public CreateCommentDtoValidator()
        {
            RuleFor(x => x.AuthorId).NotNull();
            RuleFor(x => x.PostId).NotNull();
            RuleFor(x => x.Content).NotNull();
        }
    }
}