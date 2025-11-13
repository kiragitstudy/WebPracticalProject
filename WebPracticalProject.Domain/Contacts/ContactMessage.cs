using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebPracticalProject.Domain.Contacts;

[Table("contact_messages", Schema = "app")]
public class ContactMessage
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("user_id")]
    public Guid? UserId { get; set; }   

    [Column("name")]
    public string? Name { get; set; }

    [Required]
    [Column("email")]
    public string Email { get; set; } = null!;

    [Column("subject")]
    public string? Subject { get; set; }

    [Required]
    [Column("message")]
    public string Message { get; set; } = null!;

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}