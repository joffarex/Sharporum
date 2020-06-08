using FluentValidation;
using Violetum.ApplicationCore.Dtos.Profile;

namespace Violetum.API.Validators.Profile
{
    [FluentValidator]
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.Username).NotEmpty().Matches("^[a-zA-Z0-9 ]*$");
            RuleFor(x => x.Name).NotEmpty().Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.GivenName).NotEmpty().Matches("^[a-zA-Z]*$");
            RuleFor(x => x.FamilyName).NotEmpty().Matches("^[a-zA-Z]*$");
            RuleFor(x => x.Picture).NotEmpty()
                .Matches("@(https?:)?//?[^'\"<>]+?\\.(jpg|jpeg|gif|png)@");
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.Birthdate).NotNull();
            RuleFor(x => x.Website).NotEmpty().Matches(
                "/((([A-Za-z]{3,9}:(?:\\/\\/)?)(?:[-;:&=\\+\\$,\\w]+@)?[A-Za-z0-9.-]+|(?:www.|[-;:&=\\+\\$,\\w]+@)[A-Za-z0-9.-]+)((?:\\/[\\+~%\\/.\\w-_]*)?\\??(?:[-\\+=&;%@.\\w_]*)#?(?:[\\w]*))?)/"
            );
        }
    }
}