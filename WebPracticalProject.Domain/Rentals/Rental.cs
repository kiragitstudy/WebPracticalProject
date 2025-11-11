using System.ComponentModel.DataAnnotations.Schema;
using WebPracticalProject.Domain.Catalog;
using WebPracticalProject.Domain.Common;
using WebPracticalProject.Domain.Users;

namespace WebPracticalProject.Domain.Rentals;

[Table("rentals", Schema = "app")]
public class Rental
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }
    public User? User { get; set; }

    [Column("instrument_id")]
    public Guid InstrumentId { get; set; }
    public Instrument Instrument { get; set; } = null!;

    [Column("start_at")]
    public DateTimeOffset StartAt { get; set; }

    [Column("end_at")]
    public DateTimeOffset EndAt { get; set; }

    [Column("status")]
    public RentalStatus Status { get; set; } = RentalStatus.Draft;

    [Column("total_amount")]
    public decimal? TotalAmount { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}