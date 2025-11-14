using WebPracticalProject.Service.Dto;

namespace WebPracticalProject.Service.Interfaces;

public interface IRentalService
{
    Task<RentalVm> CreateAsync(CreateRentalDto dto, CancellationToken ct);
    Task<RentalVm?> GetAsync(Guid id, CancellationToken ct);
    Task<PagedResult<RentalVm>> ListAsync(int page, int size, CancellationToken ct);
    Task<PagedResult<RentalVm>> ListMineAsync(Guid userId, int page, int size, CancellationToken ct);
    Task UpdateAsync(Guid id, UpdateRentalDto dto, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}