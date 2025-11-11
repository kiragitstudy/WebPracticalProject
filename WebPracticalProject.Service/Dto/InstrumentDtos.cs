namespace WebPracticalProject.Service.Dto;

public sealed class CreateInstrumentDto
{
    public required string Title { get; init; }
    public string? Brand { get; init; }
    public string? Category { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public required decimal PricePerDay { get; init; }
    public bool IsFeatured { get; init; } = false;
    public bool IsActive { get; init; } = true;
}
public sealed class UpdateInstrumentDto
{
    public string? Title { get; init; }
    public string? Brand { get; init; }
    public string? Category { get; init; }
    public string? Description { get; init; }
    public string? ImageUrl { get; init; }
    public decimal? PricePerDay { get; init; }
    public bool? IsFeatured { get; init; }
    public bool? IsActive { get; init; }
}
public sealed class InstrumentVm
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Brand { get; init; }
    public string? Category { get; init; }
    public decimal PricePerDay { get; init; }
    public string? ImageUrl { get; init; }
}
