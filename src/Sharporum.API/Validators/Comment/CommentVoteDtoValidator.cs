using FluentValidation;
using Sharporum.Core.Dtos.Comment;

namespace Sharporum.API.Validators.Comment
{
    public class CommentVoteDtoValidator : AbstractValidator<CommentVoteDto>
    {
        public CommentVoteDtoValidator()
        {
            RuleFor(x => x.Direction).NotNull().Must(x => x.Equals(-1) || x.Equals(0) || x.Equals(1));
        }
    }
}