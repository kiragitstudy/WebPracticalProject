using WebPracticalProject.Domain.Catalog;

namespace WebPracticalProject.DAL.Interfaces;

public interface IInstrumentRepository
{
    Task<Guid> CreateAsync(CreateInstrumentArgs args, CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateInstrumentArgs args, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<Instrument?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<Instrument> Items,int Total)> GetPagedAsync(string? category,int page,int size,CancellationToken ct);
}