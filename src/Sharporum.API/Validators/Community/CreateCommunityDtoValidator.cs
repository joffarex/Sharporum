using FluentValidation;
using Sharporum.Core.Dtos.Community;

namespace Sharporum.API.Validators.Community
{
    public class CreateCommunityDtoValidator : AbstractValidator<CreateCommunityDto>
    {
        public CreateCommunityDtoValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(10, 255).Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Description).NotEmpty();
        }
    }
}