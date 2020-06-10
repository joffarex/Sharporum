using FluentValidation;
using Violetum.ApplicationCore.Dtos.Profile;

namespace Violetum.API.Validators.Profile
{
    [FluentValidator]
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.UserName).NotEmpty().Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.FirstName).NotEmpty().Matches("^[a-zA-Z]*$");
            RuleFor(x => x.LastName).NotEmpty().Matches("^[a-zA-Z]*$");
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.Birthdate).NotNull();
        }
    }
}