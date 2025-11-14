using WebPracticalProject.DAL;
using WebPracticalProject.DAL.Interfaces;
using WebPracticalProject.Domain.Common;
using WebPracticalProject.Domain.Contracts;
using WebPracticalProject.Domain.Rentals;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Service.Services;

public sealed class RentalService(IRentalRepository rentals, IInstrumentRepository instruments) : IRentalService
{
    public async Task<RentalVm> CreateAsync(CreateRentalDto dto, CancellationToken ct)
    {
        if (dto.EndAt <= dto.StartAt) throw new ArgumentException("end <= start");
        var instr = await instruments.GetByIdAsync(dto.InstrumentId, ct) ?? throw new KeyNotFoundException("instrument");
        var days = (int)Math.Ceiling((dto.EndAt - dto.StartAt).TotalDays);
        if (days <= 0) days = 1;

        var id = await rentals.CreateAsync(
            new CreateRentalArgs(dto.UserId, dto.InstrumentId, dto.StartAt, dto.EndAt, instr.PricePerDay * days), ct);

        var r = await rentals.GetByIdAsync(id, ct)!;
        return new RentalVm { Id=r.Id, UserId=r.UserId,
            InstrumentId=r.InstrumentId, StartAt=r.StartAt, 
            EndAt=r.EndAt, Status=r.Status.ToString().ToLower(), 
            TotalAmount=r.TotalAmount,
            CreatedAt = r.CreatedAt };
    }

    public async Task<PagedResult<RentalVm>> ListMineAsync(Guid userId, int page, int size, CancellationToken ct)
    {
        var (items, total) = await rentals.GetPagedByUserAsync(userId, page, size, ct);
        return new PagedResult<RentalVm>
        {
            Page = page, Size = size, Total = total,
            Items = items.Select(r => new RentalVm
            {
                Id = r.Id,
                UserId = r.UserId,
                InstrumentId = r.InstrumentId,
                StartAt = r.StartAt,
                EndAt = r.EndAt,
                Status = r.Status.ToString().ToLower(),
                TotalAmount = r.TotalAmount,
                CreatedAt = r.CreatedAt
            }).ToList()
        };
    }
    
    public Task UpdateAsync(Guid id, UpdateRentalDto dto, CancellationToken ct) =>
        rentals.UpdateAsync(id, new UpdateRentalArgs(dto.StartAt, dto.EndAt, dto.Status, dto.TotalAmount), ct);

    public async Task<RentalVm?> GetAsync(Guid id, CancellationToken ct)
    {
        var r = await rentals.GetByIdAsync(id, ct);
        return r is null ? null : new RentalVm
        {
            Id = r.Id, UserId = r.UserId, InstrumentId = r.InstrumentId,
            StartAt = r.StartAt, EndAt = r.EndAt,
            Status = r.Status.ToString().ToLower(),
            TotalAmount = r.TotalAmount,
            CreatedAt = r.CreatedAt
        };
    }

    public async Task<PagedResult<RentalVm>> ListAsync(int page, int size, CancellationToken ct)
    {
        var (items,total) = await rentals.GetPagedAsync(page, size, ct);
        return new PagedResult<RentalVm>
        {
            Page = page, Size = size, Total = total,
            Items = items.Select(r => new RentalVm {
                Id=r.Id, UserId=r.UserId, InstrumentId=r.InstrumentId,
                StartAt=r.StartAt, EndAt=r.EndAt, Status=r.Status.ToString().ToLower(),
                TotalAmount=r.TotalAmount,
                CreatedAt = r.CreatedAt
            }).ToList()
        };
    }

    public Task DeleteAsync(Guid id, CancellationToken ct) => rentals.DeleteAsync(id, ct);
}