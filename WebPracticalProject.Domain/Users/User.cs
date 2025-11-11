using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebPracticalProject.Domain.Common;
using WebPracticalProject.Domain.Rentals;

namespace WebPracticalProject.Domain.Users;

[Table("users", Schema = "app")]
public class User
{
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("display_name")]
    public string? DisplayName { get; set; }

    [Column("role")]
    public UserRole Role { get; set; } = UserRole.Customer;

    [Column("google_id")]
    public string? GoogleId { get; set; }

    [Column("email_confirmed")]
    public bool EmailConfirmed { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column("last_login_at")]
    public DateTimeOffset? LastLoginAt { get; set; }

    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
}