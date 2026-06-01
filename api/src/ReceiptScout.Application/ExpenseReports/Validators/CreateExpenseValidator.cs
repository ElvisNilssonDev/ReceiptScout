using FluentValidation;
using ReceiptScout.Application.ExpenseReports.Dtos;

namespace ReceiptScout.Application.ExpenseReports.Validators;

public class CreateExpenseReportValidator : AbstractValidator<CreateExpenseReportDto>
{
    public CreateExpenseReportValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}