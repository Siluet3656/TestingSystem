using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestSystem.Data;
using TestSystem.Models;
using TestSystem.Models.ViewModels;
using TestSystem.Services;

namespace TestSystem.Controllers
{
    [Authorize]
    public class SubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<SubmissionsController> _logger;

        public SubmissionsController(ApplicationDbContext context, 
                                     UserManager<IdentityUser> userManager, 
                                     ILogger<SubmissionsController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult Create(int? taskId)
        {
            var model = new CreateSubmissionViewModel
            {
                ObjectiveId = taskId,
                Objectives = _context.Objectives
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
        public async Task<IActionResult> Create(CreateSubmissionViewModel model,
                                                [FromServices] SubmissionEvaluator evaluator)
        {
            _logger.LogInformation("Create POST called. ObjectiveId={id}, Language={lang}", 
                                    model.ObjectiveId, model.Language);
            
            ModelState.Remove(nameof(model.Objectives));

            if (!ModelState.IsValid)
            {
                model.Objectives = _context.Objectives
                    .Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Title
                    })
                    .ToList();

                foreach (var entry in ModelState)
                    foreach (var error in entry.Value.Errors)
                        _logger.LogWarning("Model error for {Key}: {ErrorMessage}", entry.Key, error.ErrorMessage);

                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("User not found. POST aborted.");
                return Unauthorized();
            }

            var submission = new Submission
            {
                UserId = user.Id,
                ObjectiveId = model.ObjectiveId.Value,
                Code = model.Code,
                Language = model.Language,
                Status = SubmissionStatus.Pending,
                PassedTests = 0,
                TotalTests = 0,
                Output = "",
                ErrorMessage = ""
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
        
        [Authorize(Roles = "Admin,TaskCreator")]
        public IActionResult AllSubmissions()
        {
            var submissions = _context.Submissions
                .Include(s => s.User) 
                .Include(s => s.Objective)
                .ToList();
    
            return View(submissions); 
        }
    }
}
