using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

public sealed class CatalogController(
    IInstrumentService instruments,
    IRentalService rentals
    ) : Controller
{
    /// <summary>
    /// JSON для блока «Популярно сейчас».
    /// Берём до 3 избранных активных, если мало — добираем обычными активными.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Popular(CancellationToken ct)
    {
        // Сначала пробуем найти избранные (IsFeatured = true)
        var featuredPage = await instruments.ListAsync(
            category: null,
            brand: null,
            minPrice: null,
            maxPrice: null,
            onlyActive: true,
            featured: true,
            sort: "price_asc",
            page: 1,
            size: 3,
            ct: ct);

        var result = featuredPage.Items
            .Select(x => new
            {
                id = x.Id,
                title = x.Title,
                img = x.ImageUrl,
                priceFrom = x.PricePerDay
            })
            .ToList();

        if (result.Count < 3)
        {
            var fillerPage = await instruments.ListAsync(
                category: null,
                brand: null,
                minPrice: null,
                maxPrice: null,
                onlyActive: true,
                featured: false,
                sort: "price_asc",
                page: 1,
                size: 3 - result.Count,
                ct: ct);

            result.AddRange(
                fillerPage.Items.Select(x => new
                {
                    id = x.Id,
                    title = x.Title,
                    img = x.ImageUrl,
                    priceFrom = x.PricePerDay
                }));
        }

        return Json(result);
    }

    /// <summary>
    /// Страница каталога с фильтрами и сортировкой.
    /// </summary>
    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> Index(
        string? category,
        string? brand,
        decimal? minPrice,
        decimal? maxPrice,
        string? sort,
        bool onlyActive = true,
        bool featured = false,
        int page = 1,
        int size = 9,
        CancellationToken ct = default)
    {
        var vm = await instruments.ListAsync(
            category: category,
            brand: brand,
            minPrice: minPrice,
            maxPrice: maxPrice,
            onlyActive: onlyActive,
            featured: featured,
            sort: sort,
            page: page,
            size: size,
            ct: ct);

        var categories = await instruments.GetCategoriesAsync(ct);

        // --- НОВОЕ: узнаём, какие инструменты заняты прямо сейчас ---
        var busyMap = await rentals.GetBusyUntilNowAsync(
            vm.Items.Select(i => i.Id),
            ct);

        // запихнём в ViewBag, чтобы использовать в Razor
        ViewBag.BusyUntil = busyMap;

        // и отсортируем: свободные сначала, занятые – в конце
        var busyIds = new HashSet<Guid>(busyMap.Keys);
        vm.Items = vm.Items
            .OrderBy(i => busyIds.Contains(i.Id)) // false (свободен) → 0, true (занят) → 1
            .ToList();

        ViewBag.Category = category;
        ViewBag.Brand = brand;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.Sort = sort;
        ViewBag.OnlyActive = onlyActive;
        ViewBag.Featured = featured;
        ViewBag.Categories = categories;

        return View(vm); // /Views/Catalog/Index.cshtml
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var item = await instruments.GetAsync(id, ct);
        if (item is null) return NotFound();
        return View(item); // /Views/Catalog/Details.cshtml
    }
    
    [HttpGet]
    public async Task<IActionResult> Categories(CancellationToken ct)
    {
        var page = await instruments.ListAsync(
            category: null,
            brand: null,
            minPrice: null,
            maxPrice: null,
            onlyActive: true,
            featured: false,
            sort: null,
            page: 1,
            size: 1000,
            ct: ct);
    
        var groups = page.Items
            .Where(x => !string.IsNullOrWhiteSpace(x.Category))
            .GroupBy(x => x.Category!)
            .Select(g => new
            {
                key = g.Key,             // для ссылки /Catalog?category=key
                name = g.Key,            // заголовок карточки
                description = (string?)null, // можно потом добавить норм тексты
                imageUrl = g.FirstOrDefault()?.ImageUrl,
                instrumentsCount = g.Count()
            })
            .ToList();
    
        return Json(groups);
    }
}