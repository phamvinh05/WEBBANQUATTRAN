using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartItemService _cartService;

        public OrderController(
            IOrderService orderService,
            ICartItemService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            int userId = GetCurrentUserId();
            var cartItems = await _cartService.GetAllCartItemsByUserAsync(userId);

            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "CartItem");
            }

            var model = new OrderDto
            {
                TotalAmount = cartItems.Sum(c => c.Quantity * c.ProductPrice)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderDto model)
        {
            if (!ModelState.IsValid) return View(model);

            int userId = GetCurrentUserId();
            var cartItems = await _cartService.GetAllCartItemsByUserAsync(userId);

            if (cartItems == null || !cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "CartItem");
            }

            var orderDto = new OrderDto
            {
                UserId = userId,
                FullName = model.FullName,
                Address = model.Address,
                Phone = model.Phone,
                OrderDate = DateTime.UtcNow,
                ShippingStatus = "Chờ xử lý",
                TotalAmount = cartItems.Sum(c => c.Quantity * c.ProductPrice),
                Items = cartItems.Select(c => new OrderItemDto
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.ProductPrice
                }).ToList()
            };

            int orderId = await _orderService.AddOrderAsync(orderDto);
            await _cartService.ClearCartAsync(userId);
            return RedirectToAction("Create", "Payment", new { orderId = orderId, amount = orderDto.TotalAmount });
        }



        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        private int GetCurrentUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                throw new Exception("Bạn chưa đăng nhập.");

            return userId.Value;
        }
    }
}
