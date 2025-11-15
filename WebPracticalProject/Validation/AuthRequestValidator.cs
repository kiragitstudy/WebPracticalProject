using FluentValidation;
using WebPracticalProject.Controllers;

namespace WebPracticalProject.Validation;

public class AuthRequestValidator : AbstractValidator<AccountController.LoginRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Укажите корректный email.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .MinimumLength(6).WithMessage("Пароль должен быть не короче 6 символов.");
    }
}

public class RegisterRequestValidator : AbstractValidator<AccountController.RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Имя обязательно.")
            .MinimumLength(2).WithMessage("Имя должно быть не короче 2 символов.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обязателен.")
            .EmailAddress().WithMessage("Укажите корректный email.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .MinimumLength(6).WithMessage("Пароль должен быть не короче 6 символов.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password).WithMessage("Пароли должны совпадать.");
    }
}