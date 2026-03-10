using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ICartItemService _cartService;

        public UserController(
            IUserService userService,
            IOrderService orderService,
            ICartItemService cartItemService)
        {
            _userService = userService;
            _orderService = orderService;
            _cartService = cartItemService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var users = await _userService.GetAllUserAsync();
            var user = users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View();
            }

            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(null, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("UserRole", user.Role ?? "User");

            if (user.Role == "Admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Administrator" });
            else
                return RedirectToAction("Profile");
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var hasher = new PasswordHasher<string>();
            model.PasswordHash = hasher.HashPassword(null, model.PasswordHash);
            model.CreatedAt = DateTime.Now;
            model.Role = "User";

            await _userService.AddUserAsync(model);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login");

            var cartItems = await _cartService.GetAllCartItemsByUserAsync(userId.Value);

            var orders = await _orderService.GetOrdersByUserIdAsync(userId.Value);

            ViewBag.CartItems = cartItems;
            ViewBag.Orders = orders;

            return View(user);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login");

            var hasher = new PasswordHasher<string>();
            var result = hasher.VerifyHashedPassword(null, user.PasswordHash, currentPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Mật khẩu hiện tại không đúng.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                ModelState.AddModelError("", "Mật khẩu mới phải có ít nhất 6 ký tự.");
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Xác nhận mật khẩu không khớp.");
                return View();
            }

            user.PasswordHash = hasher.HashPassword(null, newPassword);
            await _userService.UpdateUser(user);

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công.";
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> EditProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _userService.UpdateUser(model);
            TempData["SuccessMessage"] = "Cập nhật thông tin thành công.";

            return RedirectToAction("Profile");
        }


    }
}
