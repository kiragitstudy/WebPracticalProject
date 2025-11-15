using FluentValidation;
using WebPracticalProject.Service.Dto;

namespace WebPracticalProject.Validation;

public class CreateRentalDtoValidator : AbstractValidator<CreateRentalDto>
{
    public CreateRentalDtoValidator()
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