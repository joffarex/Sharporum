using FluentValidation;
using Violetum.ApplicationCore.Dtos.Profile;

namespace Violetum.API.Validators.Profile
{
    [FluentValidator]
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Username).NotNull().Length(8, 50);
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.GivenName).NotNull();
            RuleFor(x => x.FamilyName).NotNull();
            RuleFor(x => x.Picture).NotNull();
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.Birthdate).NotNull();
            RuleFor(x => x.Website).NotNull();
        }
    }
}