﻿using FluentValidation;
using Violetum.ApplicationCore.Dtos.Comment;

namespace Violetum.API.Validators.Comment
{
    [FluentValidator]
    public class UpdateCommentDtoValidator : AbstractValidator<UpdateCommentDto>
    {
        public UpdateCommentDtoValidator()
        {
            RuleFor(x => x.Id).NotNull();
            RuleFor(x => x.PostId).NotNull();
            RuleFor(x => x.Content).NotNull();
        }
    }
}