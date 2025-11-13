using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Common;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

[AutoValidateAntiforgeryToken]
public sealed class ContactController(IContactService contacts) : Controller
{
    [HttpPost]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Send([FromBody] ContactForm? dto, CancellationToken ct = default)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Message))
            return BadRequest(new { ok = false, message = "Заполните обязательные поля." });

        var id = await contacts.CreateAsync(new CreateContactDto {
            UserId = User.GetUserId(),
            Name = dto.Name,
            Email = dto.Email!,
            Subject = dto.Subject,
            Message = dto.Message!
        }, ct);

        return Json(new { ok = true, id = id.ToString("N") });
    }

    public sealed class ContactForm
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}