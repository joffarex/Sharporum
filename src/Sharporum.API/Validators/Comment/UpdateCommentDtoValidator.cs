using FluentValidation;
using Sharporum.Core.Dtos.Comment;

namespace Sharporum.API.Validators.Comment
{
    public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}