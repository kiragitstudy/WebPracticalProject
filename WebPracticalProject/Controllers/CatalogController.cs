using Microsoft.AspNetCore.Mvc;

namespace WebPracticalProject.Controllers;

public class CatalogController : Controller
{
    // GET /Catalog/Popular
    [HttpGet]
    public IActionResult Popular()
    {
        // Пустышка: имитируем топ-позиции (обычно — из БД)
        var items = new[]
        {
            new ItemDto { Title = "Электрогитара Fender", Img = "https://images.unsplash.com/photo-1505685296765-3a2736de412f?q=80&w=1200&auto=format&fit=crop", PriceFrom = 20 },
            new ItemDto { Title = "Синтезатор Korg", Img = "https://images.unsplash.com/photo-1511379938547-c1f69419868d?q=80&w=1200&auto=format&fit=crop", PriceFrom = 25 },
            new ItemDto { Title = "Саксофон альт", Img = "https://images.unsplash.com/photo-1510915361894-db8b60106cb1?q=80&w=1200&auto=format&fit=crop", PriceFrom = 22 },
        };
        return Json(items);
    }

    // public IActionResult Index() => View();
    // public IActionResult Details() => View();
}

public sealed class ItemDto
{
    public string? Title { get; set; }
    public string? Img { get; set; }
    public decimal? PriceFrom { get; set; }
}