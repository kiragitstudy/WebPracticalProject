using Microsoft.AspNetCore.Mvc;

namespace WebPracticalProject.Controllers;

public class ContactController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Send([FromBody] ContactForm? dto)
    {
        if (dto != null && (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Message)))
            return BadRequest(new { ok = false, message = "Заполните обязательные поля." });

        var id = Guid.NewGuid().ToString("N");
        return Json(new { ok = true, id });
    }
}

public sealed class ContactForm
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Subject { get; set; }
    public string? Message { get; set; }
    public bool Agree { get; set; }
}