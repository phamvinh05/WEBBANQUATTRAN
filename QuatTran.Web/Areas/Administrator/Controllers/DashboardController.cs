using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class DashboardController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public DashboardController(
            IProductService productService,
            IOrderService orderService,
            IUserService userService)
        {
            _productService = productService;
            _orderService = orderService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var totalProducts = (await _productService.GetAllProductsAsync()).Count();
            var totalOrders = (await _orderService.GetAllOrdersAsync()).Count();
            var totalUsers = (await _userService.GetAllUserAsync()).Count();

            ViewBag.TotalProducts = totalProducts;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }
    }
}
