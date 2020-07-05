﻿using FluentValidation;
using Violetum.ApplicationCore.Dtos.Category;

namespace Violetum.API.Validators.Category
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(x => x.Name).NotNull().Length(10, 255).Matches("^[a-zA-Z0-9 ]*$");
        }
    }
}