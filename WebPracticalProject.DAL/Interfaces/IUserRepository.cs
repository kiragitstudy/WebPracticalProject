using WebPracticalProject.Domain.Users;

namespace WebPracticalProject.DAL.Interfaces;

public interface IUserRepository
{
    Task<Guid> CreateAsync(CreateUserArgs args, CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateUserArgs args, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task SetLastLoginAsync(Guid id, DateTimeOffset at, CancellationToken ct);
    Task<(IReadOnlyList<User> Items,int Total)> GetPagedAsync(int page, int size, CancellationToken ct);
    Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct);
    Task SetGoogleIdAsync(Guid userId, string googleId, CancellationToken ct);
}