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
            dto.Title.Trim(), dto.Brand, dto.Category, dto.Description, dto.ImageUrl, dto.PricePerDay, dto.IsFeatured, dto.IsActive
        ), ct);
        var m = await repo.GetByIdAsync(id, ct);
        return new InstrumentVm { Id=m.Id, Title=m.Title, Description = m.Description, Brand=m.Brand, Category=m.Category, PricePerDay=m.PricePerDay, IsActive = m.IsActive, IsFeatured = m.IsFeatured, ImageUrl=m.ImageUrl };
    }

    public Task UpdateAsync(Guid id, UpdateInstrumentDto dto, CancellationToken ct) =>
        repo.UpdateAsync(id, new UpdateInstrumentArgs(dto.Title, dto.Brand, dto.Category, dto.Description, dto.ImageUrl, dto.PricePerDay, dto.IsFeatured, dto.IsActive), ct);

    public async Task<InstrumentVm?> GetAsync(Guid id, CancellationToken ct)
    {
        var m = await repo.GetByIdAsync(id, ct);
        return m is null ? null : new InstrumentVm { Id=m.Id, Title=m.Title, Description = m.Description, Brand=m.Brand, Category=m.Category, PricePerDay=m.PricePerDay, IsActive = m.IsActive, IsFeatured = m.IsFeatured, ImageUrl=m.ImageUrl };
    }

    public async Task<PagedResult<InstrumentVm>> ListAsync(string? category, int page, int size, CancellationToken ct)
    {
        var (items,total) = await repo.GetPagedAsync(category, page, size, ct);
        return new PagedResult<InstrumentVm>
        {
            Page = page, Size = size, Total = total,
            Items = items.Select(m => new InstrumentVm { Id=m.Id, Title=m.Title, Description = m.Description, Brand=m.Brand, Category=m.Category, PricePerDay=m.PricePerDay, IsActive = m.IsActive, IsFeatured = m.IsFeatured, ImageUrl=m.ImageUrl }).ToList()
        };
    }

    public Task DeleteAsync(Guid id, CancellationToken ct) => repo.DeleteAsync(id, ct);
}