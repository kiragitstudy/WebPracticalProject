using WebPracticalProject.Domain.Common;

namespace WebPracticalProject.Domain.Contracts;

public sealed record CreateInstrumentArgs(string Title, string? Brand, string? Category, string? Description, string? ImageUrl, decimal PricePerDay, bool IsFeatured, bool IsActive);
public sealed record UpdateInstrumentArgs(string? Title, string? Brand, string? Category, string? Description, string? ImageUrl, decimal? PricePerDay, bool? IsFeatured, bool? IsActive);

public sealed record CreateRentalArgs(Guid? UserId, Guid InstrumentId, DateTimeOffset StartAt, DateTimeOffset EndAt, decimal TotalAmount);
public sealed record UpdateRentalArgs(DateTimeOffset? StartAt, DateTimeOffset? EndAt, RentalStatus? Status, decimal? TotalAmount);

public sealed record CreateContactArgs(string? Name, string Email, string? Subject, string Message);
public enum InstrumentSort
{
    Default = 0,
    PriceAsc = 1,
    PriceDesc = 2,
    TitleAsc = 3,
    TitleDesc = 4
}