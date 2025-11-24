using WebPracticalProject.Domain.Common;

namespace WebPracticalProject.Domain.Users;

public sealed record CreateUserArgs(string Email, string? PasswordHash, string? DisplayName, UserRole Role, bool IsEmailConfirmed);
public sealed record UpdateUserArgs(string? DisplayName, UserRole? Role, bool? EmailConfirmed);