using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestSystem.Data;
using TestSystem.Models;
using TestSystem.Models.ViewModels;

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
            var objectivesList = _context.Objectives
                .AsNoTracking()
                .ToList(); 

            var model = new CreateSubmissionViewModel
            {
                Objectives = objectivesList
                    .Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Title
                    })
                    .ToList()
            };

            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Objectives = _context.Objectives
                    .AsNoTracking()
                    .ToList()
                    .Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Title
                    })
                    .ToList();
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);

            var submission = new Submission
            {
                UserId = user.Id,
                ObjectiveId = model.ObjectiveId,
                Code = model.Code,
                Language = model.Language,
                Status = SubmissionStatus.Pending,
                PassedTests = 0,
                TotalTests = 0,
                Output = "",
                ErrorMessage = "",
                CreatedAt = DateTime.UtcNow
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MySubmissions));
        }
        
        public IActionResult MySubmissions()
        {
            var userId = _userManager.GetUserId(User);
            var submissions = _context.Submissions
                .Where(s => s.UserId == userId)
                .Include(s => s.Objective)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return View(submissions);
        }
        
        [Authorize(Roles = "Admin")]
        public IActionResult AllSubmissions()
        {
            var submissions = _context.Submissions
                .Include(s => s.Objective)
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            return View(submissions);
        }
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Judge(int id, [FromServices] JudgeService judge)
        {
            var submission = await _context.Submissions
                .Include(s => s.Objective)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (submission == null)
                return NotFound();

            var result = await judge.RunSubmissionAsync(submission);

            submission.Status = result.Status;
            submission.PassedTests = result.Passed;
            submission.TotalTests = result.TotalTests;
            submission.Output = result.Output ?? "";
            submission.ErrorMessage = result.ErrorMessage ?? "";

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(AllSubmissions));
        }
    }
}
