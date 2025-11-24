using System.ComponentModel.DataAnnotations.Schema;
using WebPracticalProject.Domain.Users;

namespace WebPracticalProject.Domain.Emails;

[Table("email_confirmation_tokens", Schema = "app")]
public class EmailConfirmationToken
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("token")]
    public string Token { get; set; } = null!;

    [Column("expires_at")]
    public DateTimeOffset ExpiresAt { get; set; }

    [Column("used")]
    public bool Used { get; set; }

    public User User { get; set; } = null!;
}