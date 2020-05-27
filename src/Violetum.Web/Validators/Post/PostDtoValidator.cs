using FluentValidation;
using Violetum.ApplicationCore.Dtos.Post;

namespace Violetum.Web.Validators.Post
{
    public class PostDtoValidator : AbstractValidator<PostDto>
    {
        public PostDtoValidator()
        {
            RuleFor(x => x.AuthorId).NotNull();
            RuleFor(x => x.CategoryId).NotNull();
            RuleFor(x => x.Title).NotNull().Length(10, 255);
            RuleFor(x => x.Content).NotNull();
        }
    }
}