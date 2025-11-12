using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

public sealed class CatalogController(IInstrumentService instruments) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Popular(CancellationToken ct)
    {
        var page = await instruments.ListAsync(category: null, page: 1, size: 3, ct);
        var items = page.Items.Select(x => new
        {
            Title = x.Title,
            Img = x.ImageUrl,
            PriceFrom = x.PricePerDay
        });
        return Json(items);
    }

    public IActionResult Index() => View();
    public IActionResult Details(Guid id) => View(model: id);
}