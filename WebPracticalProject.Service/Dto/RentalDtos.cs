using WebPracticalProject.Domain.Common;

namespace WebPracticalProject.Service.Dto;

public sealed class CreateRentalDto
{
    public Guid? UserId { get; init; }
    public required Guid InstrumentId { get; init; }
    public required DateTimeOffset StartAt { get; init; }
    public required DateTimeOffset EndAt { get; init; }
}

public sealed class UpdateRentalDto
{
    public DateTimeOffset? StartAt { get; init; }
    public DateTimeOffset? EndAt { get; init; }
    public RentalStatus? Status { get; init; }
    public decimal? TotalAmount { get; init; }
}

public sealed class RentalVm
{
    public required Guid Id { get; init; }
    public Guid? UserId { get; init; }
    public required Guid InstrumentId { get; init; }
    public required DateTimeOffset StartAt { get; init; }
    public required DateTimeOffset EndAt { get; init; }
    public required string Status { get; init; }
    public decimal? TotalAmount { get; init; }
}