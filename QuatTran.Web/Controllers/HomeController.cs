using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public HomeController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(int? categoryId, string? keyword)
        {
            var products = await _productService.GetAllProductsAsync();
            var categories = await _categoryService.GetAllCategoryAsync();

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
                products = products.Where(p => p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase));

            ViewBag.Categories = categories;
            ViewBag.Keyword = keyword;
            ViewBag.SelectedCategory = categoryId;

            return View(products);
        }
    }
}
