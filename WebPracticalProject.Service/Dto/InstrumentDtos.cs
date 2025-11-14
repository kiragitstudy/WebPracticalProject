namespace WebPracticalProject.Service.Dto;

public sealed class CreateInstrumentDto
{
    public string Title { get; set; } = null!;
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal PricePerDay { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
}
public sealed class UpdateInstrumentDto
{
    public string? Title { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? PricePerDay { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsActive { get; set; }
}
public sealed class InstrumentVm
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? Brand { get; init; }
    public string? Category { get; init; }
    public decimal PricePerDay { get; init; }
    public bool IsFeatured { get; init; }
    public bool IsActive { get; init; } = true;
    public string? ImageUrl { get; init; }
}
