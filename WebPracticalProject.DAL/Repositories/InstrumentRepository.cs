using Microsoft.EntityFrameworkCore;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Catalog;
using WebPracticalProject.Domain.Contracts;

namespace WebPracticalProject.DAL.Repositories;

public sealed class InstrumentRepository(AppDbContext db) : IInstrumentRepository
{
    public async Task<Guid> CreateAsync(CreateInstrumentArgs a, CancellationToken ct)
    {
        var e = new Instrument { Title=a.Title, Brand=a.Brand, 
            Category=a.Category, Description=a.Description, 
            ImageUrl=a.ImageUrl, PricePerDay=a.PricePerDay, 
            IsFeatured=a.IsFeatured, IsActive=a.IsActive };
        await db.Instruments.AddAsync(e, ct);
        await db.SaveChangesAsync(ct);
        return e.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateInstrumentArgs a, CancellationToken ct)
    {
        var e = await db.Instruments.FirstOrDefaultAsync(x => x.Id == id, ct) ?? throw new KeyNotFoundException("instrument");
        if (a.Title is not null) e.Title = a.Title;
        if (a.Brand is not null) e.Brand = a.Brand;
        if (a.Category is not null) e.Category = a.Category;
        if (a.Description is not null) e.Description = a.Description;
        if (a.ImageUrl is not null) e.ImageUrl = a.ImageUrl;
        if (a.PricePerDay.HasValue) e.PricePerDay = a.PricePerDay.Value;
        if (a.IsFeatured.HasValue) e.IsFeatured = a.IsFeatured.Value;
        if (a.IsActive.HasValue) e.IsActive = a.IsActive.Value;
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        db.Attach(new Instrument { Id = id });
        db.Remove(new Instrument { Id = id });
        await db.SaveChangesAsync(ct);
    }

    public Task<Instrument?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Instruments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<(IReadOnlyList<Instrument>, int)> GetPagedAsync(string? category, int page, int size, CancellationToken ct)
    {
        var q = db.Instruments.AsNoTracking().Where(x => x.IsActive);
        if (!string.IsNullOrWhiteSpace(category)) q = q.Where(x => x.Category == category);
        q = q.OrderByDescending(x => x.CreatedAt);
        var total = await q.CountAsync(ct);
        var items = await q.Skip((page-1)*size).Take(size).ToListAsync(ct);
        return (items,total);
    }
}