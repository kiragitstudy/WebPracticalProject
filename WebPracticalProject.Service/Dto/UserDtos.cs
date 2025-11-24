using WebPracticalProject.Domain.Common;

namespace WebPracticalProject.Service.Dto;

public sealed class RegisterDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? DisplayName { get; init; }
}

public sealed class LoginDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public sealed class AuthUserVm
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string? DisplayName { get; init; }
    public required string Role { get; init; }
    public required bool IsEmailConfirmed { get; init; }
}

public sealed class CreateUserDto
{
    public required string Email { get; init; }
    public string? Password { get; init; }
    public string? DisplayName { get; init; }
    public UserRole Role { get; init; } = UserRole.Customer;
}
public sealed class UpdateUserDto
{
    public string? DisplayName { get; init; }
    public UserRole? Role { get; init; }
    public bool? EmailConfirmed { get; init; }
}

public sealed class UserVm
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public string? DisplayName { get; init; }
    public UserRole Role { get; init; } = UserRole.Customer;
}
