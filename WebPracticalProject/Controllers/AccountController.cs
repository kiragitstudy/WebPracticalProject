using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Common;
using WebPracticalProject.Domain.Common;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

[AutoValidateAntiforgeryToken]
public sealed class AccountController(IAuthService auth, IUserService users, IContactService contacts) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest? dto, CancellationToken ct = default)
    {
        if (dto is null) return BadRequest(new { ok = false, message = "Пустой запрос." });
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key.Contains('.')
                        ? kvp.Key.Split('.').Last()
                        : kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { ok = false, errors });
        }
        
        try
        {
            var vm = await auth.RegisterAsync(new RegisterDto
            {
                Email = dto.Email!,
                Password = dto.Password!,
                DisplayName = dto.DisplayName
            }, ct);

            await SignInAsync(vm); // авто-вход после регистрации

            return Json(new { ok = true, user = new { id = vm.Id, email = vm.Email, displayName = vm.DisplayName, role = vm.Role } });
        }
        catch (InvalidOperationException ex) when (ex.Message == "email_exists")
        {
            return BadRequest(new { ok = false, message = "Email уже используется." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest? dto, CancellationToken ct = default)
    {
        if (dto is null) return BadRequest(new { ok = false, message = "Пустой запрос." });
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key.Contains('.')
                        ? kvp.Key.Split('.').Last()
                        : kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { ok = false, errors });
        }

        try
        {
            var vm = await auth.LoginAsync(new LoginDto
            {
                Email = dto.Email!,
                Password = dto.Password!
            }, ct);

            await SignInAsync(vm);

            return Json(new { ok = true, user = new { id = vm.Id, email = vm.Email, displayName = vm.DisplayName, role = vm.Role } });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { ok = false, message = "Неверный email или пароль." });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Json(new { ok = true });
    }

    [HttpGet]
    public IActionResult Me()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Json(new { ok = false, user = (object?)null });

        return Json(new
        {
            ok = true,
            user = new
            {
                id = User.GetUserId()?.ToString("D"),
                email = User.GetEmail(),
                role = User.GetRole()
            }
        });
    }
    
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile(int page = 1, int size = 10, CancellationToken ct = default)
    {
        var userId = User.GetUserId() ?? Guid.Empty;
        var vm = await users.GetAsync(userId, ct);
        if (vm is null) return NotFound();
    
        var msgs = await contacts.ListMineAsync(User.GetEmail(), page, size, ct);
    
        var model = new ProfilePageVm
        {
            Email = vm.Email,
            DisplayName = vm.DisplayName,
            Role = vm.Role,
            Messages = msgs
        };
        return View(model);
    }

    // ---------- Update profile (display name) ----------
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest form, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        await users.UpdateAsync(userId.Value, new UpdateUserDto
        {
            DisplayName = string.IsNullOrWhiteSpace(form.DisplayName) ? null : form.DisplayName.Trim()
        }, ct);

        return RedirectToAction(nameof(Profile));
    }

    // ---------- Delete account (с подтверждением пароля) ----------
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteAccount([FromForm] DeleteAccountRequest form, CancellationToken ct)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();
        if (string.IsNullOrWhiteSpace(form.Password))
            return BadRequest(new { ok = false, message = "Укажите пароль." });

        // Сервис аутентификации проверит пароль и удалит (или бросит исключение)
        await auth.DeleteSelfAsync(userId.Value, form.Password, ct);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    // Helpers: создать куку аутентификации
    private async Task SignInAsync(AuthUserVm vm)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, vm.Id.ToString("D")),
            new(ClaimTypes.Email, vm.Email),
            new(ClaimTypes.Name, vm.DisplayName),
            new(ClaimTypes.Role, vm.Role) 
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var props = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
    }
    
    public sealed class ProfilePageVm
    {
        public required string Email { get; set; }
        public string? DisplayName { get; set; }
        public required UserRole Role { get; set; }
        public required PagedResult<ContactVm> Messages { get; set; }
    }
    
    public sealed class LoginRequest { 
        public string? Email { get; set; }
        public string? Password { get; set; } }
    public sealed class RegisterRequest { public string? DisplayName { get; set; } public string? Email { get; set; } public string? Password { get; set; } public string? ConfirmPassword { get; set; } }
    public sealed class UpdateProfileRequest { public string? DisplayName { get; set; } }
    public sealed class DeleteAccountRequest { public string? Password { get; set; } }
}
