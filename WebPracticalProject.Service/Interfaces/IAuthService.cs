using WebPracticalProject.Service.Dto;

namespace WebPracticalProject.Service.Interfaces;

public interface IAuthService
{
    Task<AuthUserVm> RegisterAsync(RegisterDto dto, CancellationToken ct);
    Task<AuthUserVm> LoginAsync(LoginDto dto, CancellationToken ct);
    Task DeleteSelfAsync(Guid userId, string password, CancellationToken ct);
}