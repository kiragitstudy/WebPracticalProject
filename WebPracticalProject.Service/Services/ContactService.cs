using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Contracts;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Service.Services;

public sealed class ContactService(IContactMessageRepository repo) : IContactService
{
    public async Task<Guid> CreateAsync(CreateContactDto dto, CancellationToken ct) =>
        await repo.CreateAsync(new CreateContactArgs(dto.Name, dto.Email, dto.Subject, dto.Message), ct);

    public async Task<ContactVm?> GetAsync(Guid id, CancellationToken ct)
    {
        var m = await repo.GetByIdAsync(id, ct);
        return m is null ? null : new ContactVm {
            Id=m.Id, Name=m.Name, Email=m.Email, Subject=m.Subject, Message=m.Message, CreatedAt=m.CreatedAt
        };
    }

    public async Task<PagedResult<ContactVm>> ListAsync(int page, int size, CancellationToken ct)
    {
        var (items,total) = await repo.GetPagedAsync(page, size, ct);
        return new PagedResult<ContactVm> {
            Page=page, Size=size, Total=total,
            Items = items.Select(x => new ContactVm {
                Id=x.Id, Email=x.Email, Name=x.Name, Subject=x.Subject, Message=x.Message, CreatedAt=x.CreatedAt
            }).ToList()
        };
    }

    public async Task<PagedResult<ContactVm>> ListMineAsync(string? email, int page, int size, CancellationToken ct)
    {
        var (items, total) = await repo.GetPagedByUserAsync(email, page, size, ct);
        return new PagedResult<ContactVm>
        {
            Page=page, Size=size, Total=total,
            Items = items.Select(x => new ContactVm {
                Id=x.Id, Email=x.Email, Name=x.Name, Subject=x.Subject, Message=x.Message, CreatedAt=x.CreatedAt
            }).ToList()
        };
    }

    public Task DeleteAsync(Guid id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}