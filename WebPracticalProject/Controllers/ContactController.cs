using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.DAL;
using WebPracticalProject.Domain.Contacts;

namespace WebPracticalProject.Controllers;

public class ContactController(AppDbContext db) : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send([FromBody] ContactForm? dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest(new { ok = false, message = "Заполните обязательные поля." });

        var row = new ContactMessage {
            Email = dto.Email!,
            Name = dto.Name,
            Subject = dto.Subject,
            Message = dto.Message!,
            CreatedAt = DateTimeOffset.UtcNow
        };
        db.ContactMessages.Add(row);
        await db.SaveChangesAsync();

        return Json(new { ok = true, id = row.Id.ToString("N") });
    }

    public sealed class ContactForm
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}