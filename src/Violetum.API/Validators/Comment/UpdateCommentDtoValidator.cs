using FluentValidation;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.API.Validators.Comment
{
    [FluentValidator]
    public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Content).NotEmpty();
        }
    }
}