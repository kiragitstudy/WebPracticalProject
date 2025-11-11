using WebPracticalProject.Service.Dto;

namespace WebPracticalProject.Service.Interfaces;

public interface IContactService
{
    Task<Guid> CreateAsync(CreateContactDto dto, CancellationToken ct);
    Task<ContactVm?> GetAsync(Guid id, CancellationToken ct);
    Task<PagedResult<ContactVm>> ListAsync(int page, int size, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}