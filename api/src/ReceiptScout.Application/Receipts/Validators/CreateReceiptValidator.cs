using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using ReceiptScout.Application.Receipts.Dtos;

namespace ReceiptScout.Application.Receipts.Validators;

public class CreateReceiptValidator : AbstractValidator<CreateReceiptDto>
{
    public CreateReceiptValidator()
    {
        RuleFor(x => x.Merchant)
            .NotEmpty().WithMessage("Merchant is required.")
            .MaximumLength(200);

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than zero.");

        RuleFor(x => x.VatAmount)
            .GreaterThanOrEqualTo(0).WithMessage("VAT amount cannot be negative.");

        RuleFor(x => x.Date)
            .NotEmpty();

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ImageUrl)
            .MaximumLength(500);
    }
}
