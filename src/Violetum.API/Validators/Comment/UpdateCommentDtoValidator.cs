using FluentValidation;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.API.Validators.Comment
{
    public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}