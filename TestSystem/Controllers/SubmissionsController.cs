using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestSystem.Data;
using TestSystem.Models;

namespace TestSystem.Controllers
{
    [Authorize]
    public class SubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SubmissionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string code, string language)
        {
            var user = await _userManager.GetUserAsync(User);

            var submission = new Submission
            {
                UserId = user.Id,
                Code = code,
                Language = language,
                TaskId = 1
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return RedirectToAction("MySubmissions");
        }

        public IActionResult MySubmissions()
        {
            var userId = _userManager.GetUserId(User);
            var submissions = _context.Submissions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return View(submissions);
        }
    }
}