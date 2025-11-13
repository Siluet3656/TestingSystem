using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestSystem.Data;

namespace TestSystem.Controllers
{
    [Authorize(Roles = "Admin")] // Доступ только для Admin
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // Главная страница админа
        public IActionResult Index()
        {
            return View();
        }

        // Список всех пользователей
        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
    }
}