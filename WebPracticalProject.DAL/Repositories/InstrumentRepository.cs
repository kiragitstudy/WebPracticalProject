using Microsoft.EntityFrameworkCore;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Catalog;
using WebPracticalProject.Domain.Contracts;

namespace WebPracticalProject.DAL.Repositories;

public sealed class InstrumentRepository(AppDbContext db) : IInstrumentRepository
{
    public async Task<Guid> CreateAsync(CreateInstrumentArgs a, CancellationToken ct)
    {
        var e = new Instrument
        {
            Title = a.Title,
            Brand = a.Brand,
            Category = a.Category,
            Description = a.Description,
            ImageUrl = a.ImageUrl,
            PricePerDay = a.PricePerDay,
            IsFeatured = a.IsFeatured,
            IsActive = a.IsActive
        };

        await db.Instruments.AddAsync(e, ct);
        await db.SaveChangesAsync(ct);
        return e.Id;
    }

    public async Task UpdateAsync(Guid id, UpdateInstrumentArgs a, CancellationToken ct)
    {
        var e = await db.Instruments.FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new KeyNotFoundException("instrument");

        if (a.Title is not null)        e.Title = a.Title;
        if (a.Brand is not null)        e.Brand = a.Brand;
        if (a.Category is not null)     e.Category = a.Category;
        if (a.Description is not null)  e.Description = a.Description;
        if (a.ImageUrl is not null)     e.ImageUrl = a.ImageUrl;
        if (a.PricePerDay.HasValue)     e.PricePerDay = a.PricePerDay.Value;
        if (a.IsFeatured.HasValue)      e.IsFeatured = a.IsFeatured.Value;
        if (a.IsActive.HasValue)        e.IsActive = a.IsActive.Value;

        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var stub = new Instrument { Id = id };
        db.Attach(stub);
        db.Remove(stub);
        await db.SaveChangesAsync(ct);
    }

    public Task<Instrument?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Instruments.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Расширенный GetPaged с фильтрами и сортировкой.
    /// </summary>
    public async Task<(IReadOnlyList<Instrument> Items, int Total)> GetPagedAsync(
        string? category,
        string? brand,
        decimal? minPrice,
        decimal? maxPrice,
        bool onlyActive,
        bool featured,
        InstrumentSort sort,
        int page,
        int size,
        CancellationToken ct)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 1;

        var q = db.Instruments.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            var cat = category.Trim();
            q = q.Where(x => x.Category == cat);
        }

        if (!string.IsNullOrWhiteSpace(brand))
        {
            var br = brand.Trim();
            q = q.Where(x => x.Brand == br);
        }

        if (minPrice.HasValue)
        {
            q = q.Where(x => x.PricePerDay >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            q = q.Where(x => x.PricePerDay <= maxPrice.Value);
        }

        if (onlyActive)
        {
            q = q.Where(x => x.IsActive);
        }

        if (featured)
        {
            q = q.Where(x => x.IsFeatured);
        }

        // Сортировка
        q = sort switch
        {
            InstrumentSort.PriceAsc  => q.OrderBy(x => x.PricePerDay).ThenByDescending(x => x.CreatedAt),
            InstrumentSort.PriceDesc => q.OrderByDescending(x => x.PricePerDay).ThenByDescending(x => x.CreatedAt),
            InstrumentSort.TitleAsc  => q.OrderBy(x => x.Title),
            InstrumentSort.TitleDesc => q.OrderByDescending(x => x.Title),
            _                        => q.OrderByDescending(x => x.CreatedAt)
        };

        var total = await q.CountAsync(ct);

        var items = await q
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct)
    {
        return await db.Instruments
            .AsNoTracking()
            .Where(x => x.Category != null && x.Category != "")
            .Select(x => x.Category!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);
    }
}
