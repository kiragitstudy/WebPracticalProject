using WebPracticalProject.DAL;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Catalog;
using WebPracticalProject.Domain.Contracts;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Service.Services;

public sealed class InstrumentService(IInstrumentRepository repo) : IInstrumentService
{
    
    public async Task<InstrumentVm> CreateAsync(CreateInstrumentDto dto, CancellationToken ct)
    {
        var id = await repo.CreateAsync(new CreateInstrumentArgs(
            dto.Title.Trim(),
            dto.Brand,
            dto.Category,
            dto.Description,
            dto.ImageUrl,
            dto.PricePerDay,
            dto.IsFeatured,
            dto.IsActive
        ), ct);

        var m = await repo.GetByIdAsync(id, ct)
                ?? throw new InvalidOperationException("Instrument not found after creation.");

        return ToVm(m);
    }

    public Task UpdateAsync(Guid id, UpdateInstrumentDto dto, CancellationToken ct) =>
        repo.UpdateAsync(id, new UpdateInstrumentArgs(
            dto.Title,
            dto.Brand,
            dto.Category,
            dto.Description,
            dto.ImageUrl,
            dto.PricePerDay,
            dto.IsFeatured,
            dto.IsActive
        ), ct);
    public async Task<InstrumentVm?> GetAsync(Guid id, CancellationToken ct)
    {
        var m = await repo.GetByIdAsync(id, ct);
        return m is null ? null : ToVm(m);
    }

    public async Task<PagedResult<InstrumentVm>> ListAsync(
        string? category,
        string? brand,
        decimal? minPrice,
        decimal? maxPrice,
        bool onlyActive,
        bool featured,
        string? sort,
        int page,
        int size,
        CancellationToken ct)
    {
        var sortEnum = sort switch
        {
            "price_asc"  => InstrumentSort.PriceAsc,
            "price_desc" => InstrumentSort.PriceDesc,
            "title_asc"  => InstrumentSort.TitleAsc,
            "title_desc" => InstrumentSort.TitleDesc,
            _            => InstrumentSort.Default
        };

        var (items, total) = await repo.GetPagedAsync(
            category,
            brand,
            minPrice,
            maxPrice,
            onlyActive,
            featured,
            sortEnum,
            page,
            size,
            ct);

        return new PagedResult<InstrumentVm>
        {
            Page = page,
            Size = size,
            Total = total,
            Items = items.Select(ToVm).ToList()
        };
    }


    public Task DeleteAsync(Guid id, CancellationToken ct) => repo.DeleteAsync(id, ct);
    
    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken ct)
    {
        var list = await repo.GetCategoriesAsync(ct);
        // Можно дополнительно сортировать/чистить
        return list
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .OrderBy(c => c)
            .ToArray();
    }

    private static InstrumentVm ToVm(Instrument m) => new()
    {
        Id = m.Id,
        Title = m.Title,
        Description = m.Description,
        Brand = m.Brand,
        Category = m.Category,
        PricePerDay = m.PricePerDay,
        IsActive = m.IsActive,
        IsFeatured = m.IsFeatured,
        ImageUrl = m.ImageUrl
    };
}