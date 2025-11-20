using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestSystem.Models;
using TestSystem.Data;
using TestSystem.Models.ViewModels;

namespace TestSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userProfiles = await _context.UserProfiles.ToDictionaryAsync(up => up.UserId);

            var userViewModels = users.Select(user => new UserWithProfileViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                FirstName = userProfiles.ContainsKey(user.Id) ? userProfiles[user.Id].FirstName : "Не указано",
                LastName = userProfiles.ContainsKey(user.Id) ? userProfiles[user.Id].LastName : "Не указано",
                CreatedAt = userProfiles.ContainsKey(user.Id) ? userProfiles[user.Id].CreatedAt : DateTime.MinValue,
                HasProfile = userProfiles.ContainsKey(user.Id)
            }).ToList();

            return View(userViewModels);
        }

        // Дополнительный метод для получения деталей пользователя
        public async Task<IActionResult> UserDetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == id);
            var roles = await _userManager.GetRolesAsync(user);

            var viewModel = new UserDetailViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                FirstName = profile?.FirstName ?? "Не указано",
                LastName = profile?.LastName ?? "Не указано",
                CreatedAt = profile?.CreatedAt ?? DateTime.MinValue,
                Roles = roles.ToList()
            };

            return View(viewModel);
        }
    }
}