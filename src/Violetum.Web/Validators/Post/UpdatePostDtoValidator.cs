using FluentValidation;
using Violetum.ApplicationCore.Dtos.Post;

namespace Violetum.Web.Validators.Post
{
    public class UpdatePostDtoValidator : AbstractValidator<UpdatePostDto>
    {
        public UpdatePostDtoValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.Title).NotNull().Length(10, 255);
            RuleFor(x => x.Content).NotNull();
        }
    }
}