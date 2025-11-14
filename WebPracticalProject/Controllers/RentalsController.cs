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
            // можно сразу подставить рекомендуемые даты
            StartAt = DateTimeOffset.Now.AddHours(1),
            EndAt   = DateTimeOffset.Now.AddDays(1)
        });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] CreateRentalForm form, CancellationToken ct)
    {
        var startUtc = form.StartAt.ToUniversalTime();
        var endUtc   = form.EndAt.ToUniversalTime();
        if (startUtc >= endUtc)
        {
            ModelState.AddModelError("", "Дата окончания должна быть позже даты начала.");
            var inst = await instruments.GetAsync(form.InstrumentId, ct);
            if (inst is null) return NotFound();
            return View(new CreateRentalPageVm { Instrument = inst, StartAt = form.StartAt, EndAt = form.EndAt });
        }

        try
        {
            await rentals.CreateAsync(new CreateRentalDto
            {
                UserId       = User.GetUserId()!.Value,
                InstrumentId = form.InstrumentId,
                StartAt      = startUtc, 
                EndAt        = endUtc
            }, ct);

            return RedirectToAction(nameof(My));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var inst = await instruments.GetAsync(form.InstrumentId, ct);
            if (inst is null) return NotFound();
            return View(new CreateRentalPageVm { Instrument = inst, StartAt = form.StartAt, EndAt = form.EndAt });
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
