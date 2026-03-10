using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICartItemService _cartItemService;
        private readonly ICategoryService _categoryService;

        public ProductController(
            IProductService productService,
            ICartItemService cartItemService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _cartItemService = cartItemService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId, string? keyword)
        {
            var allProducts = await _productService.GetAllProductsAsync();

            if (categoryId.HasValue)
            {
                allProducts = allProducts.Where(p => p.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                allProducts = allProducts.Where(p => p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            ViewBag.Categories = await _productService.GetAllCategoriesAsync();
            ViewBag.SelectedCategory = categoryId;
            ViewBag.Keyword = keyword;

            return View(allProducts);
        }


        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            await _cartItemService.AddToCartAsync(userId.Value, productId, quantity);

            TempData["Success"] = "Đã thêm vào giỏ hàng.";
            return RedirectToAction("Details", new { id = productId });
        }
    }
}
