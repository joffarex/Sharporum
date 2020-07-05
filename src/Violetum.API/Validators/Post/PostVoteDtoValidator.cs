using FluentValidation;
using Violetum.ApplicationCore.Dtos.Post;

namespace Violetum.API.Validators.Post
{
    public class PostVoteDtoValidator : AbstractValidator<PostVoteDto>
    {
        public PostVoteDtoValidator()
        {
            RuleFor(x => x.Direction).NotNull().Must(x => x.Equals(-1) || x.Equals(0) || x.Equals(1));
        }
    }
}