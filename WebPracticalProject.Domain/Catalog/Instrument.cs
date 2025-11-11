using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebPracticalProject.Domain.Rentals;

namespace WebPracticalProject.Domain.Catalog;

[Table("instruments", Schema = "app")]
public class Instrument
{
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("title")]
    public string Title { get; set; } = null!;

    [Column("brand")]
    public string? Brand { get; set; }

    [Column("category")]
    public string? Category { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("price_per_day")]
    public decimal PricePerDay { get; set; }

    [Column("is_featured")]
    public bool IsFeatured { get; set; }

    [Column("is_active")]
    public bool IsActive { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}