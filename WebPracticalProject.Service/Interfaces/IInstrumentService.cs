using WebPracticalProject.Service.Dto;

namespace WebPracticalProject.Service.Interfaces;

public interface IInstrumentService
{
    Task<InstrumentVm> CreateAsync(CreateInstrumentDto dto, CancellationToken ct);
    Task<InstrumentVm?> GetAsync(Guid id, CancellationToken ct);
    Task<PagedResult<InstrumentVm>> ListAsync(string? category, int page, int size, CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateInstrumentDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}