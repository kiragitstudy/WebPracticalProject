using WebPracticalProject.Service.Dto;

namespace WebPracticalProject.Service.Interfaces;

public interface IInstrumentService
{
    Task<InstrumentVm> CreateAsync(CreateInstrumentDto dto, CancellationToken ct);
    Task<InstrumentVm?> GetAsync(Guid id, CancellationToken ct);
    Task<PagedResult<InstrumentVm>> ListAsync(
        string? category,
        string? brand,
        decimal? minPrice,
        decimal? maxPrice,
        bool onlyActive,
        bool featured,
        string? sort,
        int page,
        int size,
        CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateInstrumentDto dto, CancellationToken ct);
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}