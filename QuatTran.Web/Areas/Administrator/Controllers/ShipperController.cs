using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;

[Area("Administrator")]
public class ShipperController : Controller
{
    private readonly IShipperService _shipperService;

    public ShipperController(IShipperService shipperService)
    {
        _shipperService = shipperService;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _shipperService.GetAllShippersAsync();
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ShipperDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        await _shipperService.AddShipperAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var shipper = await _shipperService.GetShipperByIdAsync(id);
        if (shipper == null) return NotFound();
        return View(shipper);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ShipperDto dto)
    {
        if (!ModelState.IsValid) return View(dto);
        await _shipperService.UpdateShipper(dto);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var shipper = await _shipperService.GetShipperByIdAsync(id);
        if (shipper == null) return NotFound();
        return View(shipper);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _shipperService.DeleteShipperAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
