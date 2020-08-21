using FluentValidation;
using Sharporum.Core.Dtos.Community;

namespace Sharporum.API.Validators.Community
{
    public class UpdateCommunityImageDtoValidator : AbstractValidator<UpdateCommunityImageDto>
    {
        public UpdateCommunityImageDtoValidator()
        {
            RuleFor(x => x.Image).NotEmpty()
                .Matches("@(https?:)?//?[^'\"<>]+?\\.(jpg|jpeg|gif|png)@");
        }
    }
}