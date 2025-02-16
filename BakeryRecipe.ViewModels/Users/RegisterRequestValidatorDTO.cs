﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakeryRecipe.ViewModels.Users
{
    public class RegisterRequestValidatorDTO: AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestValidatorDTO()
        {
            RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email is required!").EmailAddress();
            RuleFor(x => x.Password).NotNull().NotEmpty().WithMessage("Password is required!");
        }
    }
}
