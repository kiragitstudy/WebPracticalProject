using System.Security.Claims;

namespace WebPracticalProject.Common;

public static class ClaimsExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(id, out var g) ? g : null;
    }

    public static string? GetEmail(this ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.Email);
    public static string? GetRole(this ClaimsPrincipal user)  => user.FindFirstValue(ClaimTypes.Role);
}