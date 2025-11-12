using Microsoft.EntityFrameworkCore;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Contacts;
using WebPracticalProject.Domain.Contracts;

namespace WebPracticalProject.DAL.Repositories;

public sealed class ContactMessageRepository(AppDbContext db) : IContactMessageRepository
{
    public async Task<Guid> CreateAsync(CreateContactArgs a, CancellationToken ct)
    {
        var e = new ContactMessage { Name=a.Name, Email=a.Email, Subject=a.Subject, Message=a.Message };
        await db.ContactMessages.AddAsync(e, ct);
        await db.SaveChangesAsync(ct);
        return e.Id;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        db.Attach(new ContactMessage { Id = id });
        db.Remove(new ContactMessage { Id = id });
        await db.SaveChangesAsync(ct);
    }

    public Task<ContactMessage?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.ContactMessages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<(IReadOnlyList<ContactMessage>, int)> GetPagedAsync(int page,int size,CancellationToken ct)
    {
        var q = db.ContactMessages.AsNoTracking().OrderByDescending(x => x.CreatedAt);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page-1)*size).Take(size).ToListAsync(ct);
        return (items,total);
    }
}