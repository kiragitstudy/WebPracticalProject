using WebPracticalProject.Domain.Common;

namespace WebPracticalProject.Service.Dto;

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
    public string Role { get; init; } = "customer";
}
