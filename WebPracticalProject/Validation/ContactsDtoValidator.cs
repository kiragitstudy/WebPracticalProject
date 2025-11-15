using FluentValidation;
using WebPracticalProject.Controllers;

namespace WebPracticalProject.Validation;

public sealed class CreateRentalFormValidator : AbstractValidator<RentalsController.CreateRentalForm>
{
    public CreateRentalFormValidator()
    {
        RuleFor(x => x.InstrumentId)
            .NotEmpty().WithMessage("Инструмент обязателен.");

        RuleFor(x => x.StartAt)
            .NotEqual(default(DateTimeOffset)).WithMessage("Укажите дату начала.");

        RuleFor(x => x.EndAt)
            .NotEqual(default(DateTimeOffset)).WithMessage("Укажите дату окончания.")
            .GreaterThan(x => x.StartAt).WithMessage("Дата окончания должна быть позже даты начала.");
    }
}