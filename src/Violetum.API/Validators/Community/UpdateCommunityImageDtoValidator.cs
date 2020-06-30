using FluentValidation;
using Violetum.ApplicationCore.Dtos.Community;

namespace Violetum.API.Validators.Community
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