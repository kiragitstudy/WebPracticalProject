using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

[AutoValidateAntiforgeryToken]
public sealed class AccountController(IAuthService auth) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest? dto, CancellationToken ct = default)
    {
        if (dto is null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { ok = false, message = "Заполните email и пароль." });
        if (!string.Equals(dto.Password, dto.ConfirmPassword))
            return BadRequest(new { ok = false, message = "Пароли не совпадают." });

        try
        {
            var vm = await auth.RegisterAsync(new RegisterDto
            {
                Email = dto.Email!,
                Password = dto.Password!,
                DisplayName = dto.DisplayName
            }, ct);

            // Единый JSON-ответ (как и было в твоей заглушке)
            return Json(new { ok = true, user = new { id = vm.Id, email = vm.Email, displayName = vm.DisplayName, role = vm.Role } });
        }
        catch (InvalidOperationException ex) when (ex.Message == "email_exists")
        {
            return BadRequest(new { ok = false, message = "Email уже используется." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest? dto,  CancellationToken ct = default)
    {
        
        if (dto is null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            return BadRequest(new { ok = false, message = "Укажите email и пароль." });

        try
        {
            var vm = await auth.LoginAsync(new LoginDto
            {
                Email = dto.Email!,
                Password = dto.Password!
            }, ct);

            return Json(new { ok = true, user = new { id = vm.Id, email = vm.Email, displayName = vm.DisplayName, role = vm.Role } });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { ok = false, message = "Неверный email или пароль." });
        }
    }

    public sealed class LoginRequest
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public sealed class RegisterRequest
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
