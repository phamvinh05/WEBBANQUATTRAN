using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.DTOs;

public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<IActionResult> Index()
    {
        var payments = await _paymentService.GetAllPaymentsAsync();
        return View(payments);
    }

    public async Task<IActionResult> Details(int id)
    {
        var payment = await _paymentService.GetByIdAsync(id);
        if (payment == null) return NotFound();
        return View(payment);
    }

    public IActionResult Create(int orderId, decimal amount)
    {
        var dto = new PaymentDto
        {
            OrderId = orderId,
            Amount = amount
        };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PaymentDto dto)
    {
        dto.PaymentDate = DateTime.UtcNow;
        if (dto.PaymentMethod == "BankTransfer")
            dto.PaymentStatus = "Chờ chuyển khoản";
        else
            dto.PaymentStatus = "Đã thanh toán";

        await _paymentService.AddPaymentAsync(dto);

        TempData["SuccessMessage"] = "Thanh toán thành công!";
        return RedirectToAction("Index", "Home");
    }


}
