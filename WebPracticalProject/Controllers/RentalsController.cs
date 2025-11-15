// Controllers/RentalsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPracticalProject.Common;
using WebPracticalProject.Service.Dto;
using WebPracticalProject.Service.Interfaces;

namespace WebPracticalProject.Controllers;

[AutoValidateAntiforgeryToken]
public sealed class RentalsController(IRentalService rentals, IInstrumentService instruments) : Controller
{
    // Страница оформления аренды — доступна всем (анонимам показываем призыв войти)
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Create(Guid instrumentId, CancellationToken ct)
    {
        var inst = await instruments.GetAsync(instrumentId, ct);
        if (inst is null) return NotFound();

        return View(new CreateRentalPageVm
        {
            Instrument = inst,
            StartAt = DateTimeOffset.Now.AddHours(1),
            EndAt   = DateTimeOffset.Now.AddDays(1)
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateRentalForm? dto, CancellationToken ct)
    {
        if (dto is null)
        {
            return BadRequest(new
            {
                ok = false,
                message = "Пустое тело запроса."
            });
        }
    
        // Стандартная проверка модели — как в Register
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
    
        var instrument = await instruments.GetAsync(dto.InstrumentId, ct);
        if (instrument is null)
        {
            return NotFound(new
            {
                ok = false,
                message = "Инструмент не найден или недоступен."
            });
        }
    
        var startUtc = dto.StartAt.ToUniversalTime();
        var endUtc   = dto.EndAt.ToUniversalTime();
    
        try
        {
            await rentals.CreateAsync(new CreateRentalDto
            {
                UserId       = User.GetUserId()!.Value,
                InstrumentId = dto.InstrumentId,
                StartAt      = startUtc,
                EndAt        = endUtc
            }, ct);
    
            return Json(new { ok = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                ok = false,
                message = ex.Message
            });
        }
        catch (Exception)
        {
            return BadRequest(new
            {
                ok = false,
                message = "Не удалось оформить аренду. Попробуйте позже."
            });
        }
    }


    // «Мои аренды»
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> My(int page = 1, int size = 10, CancellationToken ct = default)
    {
        var userId = User.GetUserId()!.Value;
        var myRentals = await rentals.ListMineAsync(userId, page, size, ct);

        var map = new Dictionary<Guid, InstrumentVm>();
        foreach (var id in myRentals.Items.Select(x => x.InstrumentId).Distinct())
        {
            var it = await instruments.GetAsync(id, ct);
            if (it != null) map[id] = it;
        }

        var vm = new MyPageVm { Rentals = myRentals, Instruments = map };
        return View(vm); // Views/Rentals/My.cshtml
    }

    public sealed class MyPageVm
    {
        public required PagedResult<RentalVm> Rentals { get; set; }
        public required Dictionary<Guid, InstrumentVm> Instruments { get; set; }
    }

    // ---------------- локальные модели для View ----------------

    public sealed class CreateRentalForm
    {
        public Guid InstrumentId { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }
    }

    public sealed class CreateRentalPageVm
    {
        public required InstrumentVm Instrument { get; set; }
        public DateTimeOffset? StartAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
    }
}
