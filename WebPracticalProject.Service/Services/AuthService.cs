using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Common;
using WebPracticalProject.Domain.Users;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;
using WebPracticalProject.Service.Security;

namespace WebPracticalProject.Service.Services;

public sealed class AuthService(IUserRepository users, IPasswordService passwords) : IAuthService
{
    public async Task<AuthUserVm> RegisterAsync(RegisterDto dto, CancellationToken ct)
    {
        // 1) простая проверка уникальности
        if (await users.EmailExistsAsync(dto.Email.Trim(), ct))
            throw new InvalidOperationException("email_exists");

        // 2) хеш пароля
        var hash = passwords.Hash(dto.Password);

        // 3) создать пользователя с ролью Customer
        var id = await users.CreateAsync(
            new CreateUserArgs(dto.Email.Trim(), hash, dto.DisplayName, UserRole.Customer),
            ct);

        // 4) прочитать и вернуть VM
        var u = await users.GetByIdAsync(id, ct) ?? throw new InvalidOperationException("user_not_found");
        return new AuthUserVm
        {
            Id = u.Id,
            Email = u.Email,
            DisplayName = u.DisplayName,
            Role = u.Role.ToString().ToLower()
        };
    }

    public async Task<AuthUserVm> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        var email = dto.Email.Trim();
        var u = await users.GetByEmailAsync(email, ct);
        if (u is null || string.IsNullOrEmpty(u.PasswordHash))
            throw new UnauthorizedAccessException("invalid_credentials");

        // проверка хеша
        if (!passwords.Verify(dto.Password, u.PasswordHash))
            throw new UnauthorizedAccessException("invalid_credentials");

        // обновление last_login_at
        await users.SetLastLoginAsync(u.Id, DateTimeOffset.UtcNow, ct);

        return new AuthUserVm
        {
            Id = u.Id,
            Email = u.Email,
            DisplayName = u.DisplayName,
            Role = u.Role.ToString().ToLower()
        };
    }
    
    public async Task DeleteSelfAsync(Guid userId, string password, CancellationToken ct)
    {
        var u = await users.GetByIdAsync(userId, ct) ?? throw new UnauthorizedAccessException();
        if (string.IsNullOrEmpty(u.PasswordHash) || !passwords.Verify(password, u.PasswordHash))
            throw new UnauthorizedAccessException();

        await users.DeleteAsync(userId, ct);
    }
}
