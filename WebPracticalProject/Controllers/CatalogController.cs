using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

public sealed class CatalogController(IInstrumentService instruments) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Popular(CancellationToken ct)
    {
        var page = await instruments.ListAsync(category: null, page: 1, size: 24, ct);
        var featured = page.Items
            .Where(x => x.IsActive && x.IsFeatured)
            .Take(3)
            .Select(x => new { Id = x.Id, Title = x.Title, Img = x.ImageUrl, PriceFrom = x.PricePerDay })
            .ToList();

        if (featured.Count < 3)
        {
            var filler = page.Items
                .Where(x => x.IsActive)
                .Take(3 - featured.Count)
                .Select(x => new { Id = x.Id, Title = x.Title, Img = x.ImageUrl, PriceFrom = x.PricePerDay });
            featured.AddRange(filler);
        }
        return Json(featured);
    }


    public async Task<IActionResult> Index(string? category, int page = 1, int size = 9, CancellationToken ct = default)
    {
        var vm = await instruments.ListAsync(category, page, size, ct);
        ViewBag.Category = category;
        return View(vm); // /Views/Catalog/Index.cshtml
    }

    // Детальная страница инструмента
    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var item = await instruments.GetAsync(id, ct);
        if (item is null) return NotFound();
        return View(item); // /Views/Catalog/Details.cshtml
    }
}