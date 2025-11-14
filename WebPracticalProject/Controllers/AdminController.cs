
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class AdminController(IUserService users, IContactService contacts, IInstrumentService instruments) : Controller
{
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> Users(int page = 1, int size = 20, CancellationToken ct = default)
    {
        var vm = await users.ListAsync(page, size, ct);
        return View(vm); // Views/Admin/Users.cshtml
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UpdateUserDto dto, CancellationToken ct)
    {
        await users.UpdateAsync(id, dto, ct);
        return RedirectToAction(nameof(Users));
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        await users.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Users));
    }

    // --- Сообщения (admin + manager) ---
    [Authorize(Roles = "admin,manager")]
    [HttpGet]
    public async Task<IActionResult> Messages(int page = 1, int size = 20, CancellationToken ct = default)
    {
        var vm = await contacts.ListAsync(page, size, ct);
        return View(vm); // Views/Admin/Messages.cshtml
    }

    [Authorize(Roles = "admin,manager")]
    [HttpPost]
    public async Task<IActionResult> DeleteMessage(Guid id, CancellationToken ct)
    {
        await contacts.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Messages));
    }
    
    [Authorize(Roles = "admin,manager")]
    [HttpGet]
    public async Task<IActionResult> Instruments(int page = 1, int size = 20, CancellationToken ct = default)
    {
        var vm = await instruments.ListAsync(category: null, page, size, ct);
        return View(vm); // Views/Admin/Instruments.cshtml
    }
    
    [Authorize(Roles = "admin,manager")]
    [HttpPost]
    public async Task<IActionResult> CreateInstrument([FromForm] CreateInstrumentDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("title");
        await instruments.CreateAsync(dto, ct);
        return RedirectToAction(nameof(Instruments));
    }
    
    [Authorize(Roles = "admin,manager")]
    [HttpPost]
    public async Task<IActionResult> UpdateInstrument(Guid id, [FromForm] UpdateInstrumentDto dto, CancellationToken ct)
    {
        await instruments.UpdateAsync(id, dto, ct);
        return RedirectToAction(nameof(Instruments));
    }
    
    [Authorize(Roles = "admin,manager")]
    [HttpPost]
    public async Task<IActionResult> DeleteInstrument(Guid id, CancellationToken ct)
    {
        await instruments.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Instruments));
    }
    
}