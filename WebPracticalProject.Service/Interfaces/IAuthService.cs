using System.Security.Claims;
using WebPracticalProject.Service.Dto;

namespace WebPracticalProject.Service.Interfaces;

public interface IAuthService
{
    Task<AuthUserVm> RegisterAsync(RegisterDto dto, CancellationToken ct);
    Task<AuthUserVm> LoginAsync(LoginDto dto, CancellationToken ct);
    Task<bool> ConfirmEmailAsync(string token);
    Task DeleteSelfAsync(Guid userId, string password, CancellationToken ct);
    
    Task<ClaimsPrincipal> SignInWithGoogleAsync(
        string email,
        string displayName,
        string googleId,
        CancellationToken ct);
}