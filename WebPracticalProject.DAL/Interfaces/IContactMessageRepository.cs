using WebPracticalProject.Domain.Contacts;

namespace WebPracticalProject.DAL.Interfaces;

public interface IContactMessageRepository
{
    Task<Guid> CreateAsync(CreateContactArgs args, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<ContactMessage?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<(IReadOnlyList<ContactMessage> Items,int Total)> GetPagedAsync(int page,int size,CancellationToken ct);
}