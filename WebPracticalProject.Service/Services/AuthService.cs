using System.Security.Claims;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Common;
using WebPracticalProject.Domain.Emails;
using WebPracticalProject.Domain.Users;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;
using WebPracticalProject.Service.Security;

namespace WebPracticalProject.Service.Services;

public sealed class AuthService(
    IUserRepository users, 
    IPasswordService passwords,
    IEmailSender emailSender,
    IEmailConfirmationRepository emailTokens
    ) : IAuthService
{
    public async Task<AuthUserVm> RegisterAsync(RegisterDto dto, CancellationToken ct)
    {
        var email = dto.Email.Trim();

        if (await users.EmailExistsAsync(email, ct))
            throw new InvalidOperationException("email_exists");

        var hash = passwords.Hash(dto.Password);

        var id = await users.CreateAsync(
            new CreateUserArgs(email, hash, dto.DisplayName, UserRole.Customer, IsEmailConfirmed: false),
            ct);

        var u = await users.GetByIdAsync(id, ct) 
                ?? throw new InvalidOperationException("user_not_found");

        var token = GenerateToken();

        var entity = new EmailConfirmationToken
        {
            Id = Guid.NewGuid(),
            UserId = u.Id,
            Token = token,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1),
            Used = false
        };

        await emailTokens.AddAsync(entity);
        await emailTokens.SaveChangesAsync();
        await emailSender.SendConfirmationEmailAsync(u.Email, token);

        return new AuthUserVm
        {
            Id = u.Id,
            Email = u.Email,
            DisplayName = u.DisplayName,
            Role = u.Role.ToString().ToLower(),
            IsEmailConfirmed = u.EmailConfirmed
        };
    }
    
    public async Task<ClaimsPrincipal> SignInWithGoogleAsync(
        string email,
        string displayName,
        string googleId,
        CancellationToken ct)
    {
        var user = await users.GetByGoogleIdAsync(googleId, ct);

        if (user is null)
        {
            user = await users.GetByEmailAsync(email, ct);
            if (user is null)
            {
                var id = await users.CreateAsync(
                    new CreateUserArgs(
                        Email: email,
                        PasswordHash: null,
                        DisplayName: displayName,
                        Role: UserRole.Customer,
                        IsEmailConfirmed: true
                    ),
                    ct);

                user = await users.GetByIdAsync(id, ct)
                       ?? throw new InvalidOperationException("user_not_found_after_create");
            }
            await users.SetGoogleIdAsync(user.Id, googleId, ct);
        }

        await users.SetLastLoginAsync(user.Id, DateTimeOffset.UtcNow, ct);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.DisplayName ?? user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString().ToLowerInvariant())
        };

        var identity = new ClaimsIdentity(claims, "Cookies");
        var principal = new ClaimsPrincipal(identity);
        return principal;
    }

    public async Task<AuthUserVm> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        var email = dto.Email.Trim();
        var u = await users.GetByEmailAsync(email, ct);
        if (u is null || string.IsNullOrEmpty(u.PasswordHash))
            throw new UnauthorizedAccessException("invalid_credentials");

        if (!passwords.Verify(dto.Password, u.PasswordHash))
            throw new UnauthorizedAccessException("invalid_credentials");

        await users.SetLastLoginAsync(u.Id, DateTimeOffset.UtcNow, ct);

        return new AuthUserVm
        {
            Id = u.Id,
            Email = u.Email,
            DisplayName = u.DisplayName,
            Role = u.Role.ToString().ToLower(),
            IsEmailConfirmed = u.EmailConfirmed
        };
    }

    public async Task DeleteSelfAsync(Guid userId, string password, CancellationToken ct)
    {
        var u = await users.GetByIdAsync(userId, ct) ?? throw new UnauthorizedAccessException();
        if (string.IsNullOrEmpty(u.PasswordHash) || !passwords.Verify(password, u.PasswordHash))
            throw new UnauthorizedAccessException();

        await users.DeleteAsync(userId, ct);
    }

    public async Task<bool> ConfirmEmailAsync(string token)
    {
        var entity = await emailTokens.GetByTokenAsync(token);
        if (entity == null || entity.ExpiresAt < DateTimeOffset.UtcNow)
            return false;

        entity.Used = true;
        entity.User.EmailConfirmed = true;

        await emailTokens.SaveChangesAsync();
        return true;
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}
