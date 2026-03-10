using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Controllers
{
    public class CartItemController : Controller
    {
        private readonly ICartItemService _cartService;

        public CartItemController(ICartItemService cartService)
        {
            _cartService = cartService;
        }
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var items = await _cartService.GetAllCartItemsByUserAsync(userId);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity)
        {
            int userId = GetCurrentUserId();

            var dto = new CartItemDto
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };

            await _cartService.AddToCartAsync(dto);
            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            await _cartService.UpdateQuantityAsync(cartItemId, quantity);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartService.RemoveItemAsync(cartItemId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            int userId = GetCurrentUserId();
            await _cartService.ClearCartAsync(userId);
            return RedirectToAction("Index");
        }

        private int GetCurrentUserId()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
                return HttpContext.Session.GetInt32("UserId").Value;
            throw new Exception("Chưa đăng nhập.");
        }
    }
}
