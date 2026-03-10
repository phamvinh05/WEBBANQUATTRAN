using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.Interfaces;
using QuatTran.Application.Services;

namespace QuatTran.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShipperService _shipperService;

        public OrderController(IOrderService orderService, IShipperService shipperService)
        {
            _orderService = orderService;
            _shipperService = shipperService; 
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            await _orderService.UpdateOrderStatusAsync(id, status);
            TempData["Success"] = "Cập nhật trạng thái thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }
        public async Task<IActionResult> AssignShipper(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var shippers = await _shipperService.GetAllShippersAsync();

            ViewBag.Shippers = shippers;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignShipper(int id, int shipperId)
        {
            await _orderService.UpdateOrderShipperAsync(id, shipperId);
            TempData["Success"] = "Đã gán shipper thành công.";
            return RedirectToAction("Details", new { id });
        }

    }
}
