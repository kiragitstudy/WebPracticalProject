using WebPracticalProject.Domain.Catalog;
using WebPracticalProject.Domain.Contracts;

namespace WebPracticalProject.DAL.Interfaces;

public interface IInstrumentRepository
{
    Task<Guid> CreateAsync(CreateInstrumentArgs args, CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateInstrumentArgs args, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<Instrument?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<Instrument> Items, int Total)> GetPagedAsync(
        string? category,
        string? brand,
        decimal? minPrice,
        decimal? maxPrice,
        bool onlyActive,
        bool featured,
        InstrumentSort sort,
        int page,
        int size,
        CancellationToken ct);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct);
}