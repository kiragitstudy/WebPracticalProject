
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

[Authorize]
[AutoValidateAntiforgeryToken]
public sealed class AdminController(IUserService users, IContactService contacts) : Controller
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
        await contacts.DeleteAsync(id, ct); // добавь в IContactService метод DeleteAsync, в репозитории он уже есть
        return RedirectToAction(nameof(Messages));
    }
}