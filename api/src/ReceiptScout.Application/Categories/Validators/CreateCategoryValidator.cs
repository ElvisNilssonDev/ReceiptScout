using FluentValidation;
using ReceiptScout.Application.Categories.Dtos;

namespace ReceiptScout.Application.Categories.Validators;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.BasAccount).NotEmpty().MaximumLength(10);
        RuleFor(x => x.VatRate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1);
    }
}