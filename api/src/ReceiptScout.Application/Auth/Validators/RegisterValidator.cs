using System;
using System.Collections.Generic;
using System.Text;

using FluentValidation;
using ReceiptScout.Application.Auth.Dtos;

namespace ReceiptScout.Application.Auth.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}
