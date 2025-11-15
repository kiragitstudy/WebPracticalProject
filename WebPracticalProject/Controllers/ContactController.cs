using Microsoft.AspNetCore.Mvc;
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
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key.Contains('.')
                        ? kvp.Key.Split('.').Last()
                        : kvp.Key,
                    kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { ok = false, errors });
        }
        
        var id = await contacts.CreateAsync(new CreateContactDto {
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