using Microsoft.EntityFrameworkCore;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Rentals;

namespace WebPracticalProject.DAL.Repositories;

public sealed class RentalRepository(AppDbContext db) : IRentalRepository
{
    public async Task<Guid> CreateAsync(CreateRentalArgs a, CancellationToken ct)
    {
        var e = new Rental { UserId=a.UserId, InstrumentId=a.InstrumentId, StartAt=a.StartAt, EndAt=a.EndAt, Status=Domain.Common.RentalStatus.Draft, TotalAmount=a.TotalAmount };
        await db.Rentals.AddAsync(e, ct);
        await db.SaveChangesAsync(ct);
        return e.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateRentalArgs a, CancellationToken ct)
    {
        var e = await db.Rentals.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KeyNotFoundException("rental");
        if (a.StartAt.HasValue) e.StartAt = a.StartAt.Value;
        if (a.EndAt.HasValue) e.EndAt = a.EndAt.Value;
        if (a.Status.HasValue) e.Status = a.Status.Value;
        if (a.TotalAmount.HasValue) e.TotalAmount = a.TotalAmount.Value;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        db.Attach(new Rental { Id = id });
        db.Remove(new Rental { Id = id });
        await db.SaveChangesAsync(ct);
    }

    public Task<Rental?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Rentals.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<(IReadOnlyList<Rental>, int)> GetPagedAsync(int page,int size,CancellationToken ct)
    {
        var q = db.Rentals.AsNoTracking().OrderByDescending(x => x.CreatedAt);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page-1)*size).Take(size).ToListAsync(ct);
        return (items,total);
    }
}