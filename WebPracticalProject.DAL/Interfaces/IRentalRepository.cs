using WebPracticalProject.Domain.Contracts;
using WebPracticalProject.Domain.Rentals;

namespace WebPracticalProject.DAL.Interfaces;

public interface IRentalRepository
{
    Task<Guid> CreateAsync(CreateRentalArgs args, CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateRentalArgs args, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<Rental?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<Rental> Items,int Total)> GetPagedAsync(int page, int size, CancellationToken ct);
    Task<(IReadOnlyList<Rental> Items,int Total)> GetPagedByUserAsync(Guid userId, int page, int size, CancellationToken ct);
    Task<bool> ExistsOverlapAsync(Guid instrumentId, DateTimeOffset start, DateTimeOffset end, CancellationToken ct);

}