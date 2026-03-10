using Microsoft.AspNetCore.Mvc;
using QuatTran.Application.DTOs;
using QuatTran.Application.Interfaces;

namespace QuatTran.Web.Areas.Administrator.Controllers
{
    [Area("Administrator")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUserAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        public IActionResult Create()
        {
            return View(new UserDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserDto model)
        {
            if (!ModelState.IsValid) return View(model);

            model.CreatedAt = DateTime.Now;
            if (string.IsNullOrEmpty(model.Role))
                model.Role = "User";
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<UserDto>();
            model.PasswordHash = hasher.HashPassword(model, model.PasswordHash);

            await _userService.AddUserAsync(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingUser = await _userService.GetByIdAsync(model.UserId);
            if (existingUser == null) return NotFound();
            if (string.IsNullOrWhiteSpace(model.PasswordHash))
            {
                model.PasswordHash = existingUser.PasswordHash;
            }
            else
            {
                var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<UserDto>();
                model.PasswordHash = hasher.HashPassword(model, model.PasswordHash);
            }

            await _userService.UpdateUser(model);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User", new { area = "" }); 
            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login", "User", new { area = "" });

            return View(user);
        }

        public async Task<IActionResult> EditProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User", new { area = "" });

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
                return RedirectToAction("Login", "User", new { area = "" });

            return View(user);
        }

        // POST: /Administrator/User/EditProfile
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
