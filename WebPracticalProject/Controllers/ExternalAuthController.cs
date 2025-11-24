using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

[AllowAnonymous]
public class ExternalAuthController(IAuthService authService) : Controller
{
    [HttpGet]
    public IActionResult GoogleLogin(string? returnUrl = "/")
    {
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleCallback), new { returnUrl })
        };

        return Challenge(props, "Google");
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback(string? returnUrl = "/", CancellationToken ct = default)
    {
        // читаем External-cookie, который положил Google
        var result = await HttpContext.AuthenticateAsync("External");
        if (!result.Succeeded)
        {
            var failUrl = QueryHelpers.AddQueryString("/", "login", "google_fail");
            return Redirect(failUrl);
        }

        var externalPrincipal = result.Principal!;
        var email = externalPrincipal.FindFirst(ClaimTypes.Email)?.Value;
        var name = externalPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        var googleId = externalPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(googleId))
        {
            await HttpContext.SignOutAsync("External");
            var failUrl = QueryHelpers.AddQueryString("/", "login", "google_fail");
            return Redirect(failUrl);
        }

        // маппим на нашего пользователя и создаём наш principal
        var appPrincipal =
            await authService.SignInWithGoogleAsync(email, name ?? email, googleId, ct);

        await HttpContext.SignOutAsync("External");

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            appPrincipal);

        var safeReturn = string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl;
        var okUrl = QueryHelpers.AddQueryString(safeReturn, "login", "google_ok");
        return Redirect(okUrl);
    }
}
