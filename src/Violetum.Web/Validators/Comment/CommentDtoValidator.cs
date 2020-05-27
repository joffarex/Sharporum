using FluentValidation;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.Web.Validators.Comment
{
    public class CommentDtoValidator : AbstractValidator<CommentDto>
    {
        public CommentDtoValidator()
        {
            RuleFor(x => x.AuthorId).NotNull();
            RuleFor(x => x.PostId).NotNull();
            RuleFor(x => x.Content).NotNull();
        }
    }
}