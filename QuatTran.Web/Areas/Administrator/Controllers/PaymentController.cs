using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
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
    }
}
