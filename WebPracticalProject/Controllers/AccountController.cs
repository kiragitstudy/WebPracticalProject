using Microsoft.AspNetCore.Mvc;

namespace WebPracticalProject.Controllers;

public class AccountController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login([FromBody] LoginRequest? dto)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { ok = false, message = "Укажите email и пароль." });

        // Пустышка: «успешный вход»
        var user = new { displayName = dto.Email.Split('@').FirstOrDefault() ?? "User" };
        return Json(new { ok = true, user });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register([FromBody] RegisterRequest? dto)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { ok = false, message = "Заполните email и пароль." });

        if (!string.Equals(dto.Password, dto.ConfirmPassword))
            return BadRequest(new { ok = false, message = "Пароли не совпадают." });

        // Пустышка: «создали аккаунт»
        return Json(new { ok = true });
    }
}

public sealed class LoginRequest
{
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool? RememberMe { get; set; }
}

public sealed class RegisterRequest
{
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}