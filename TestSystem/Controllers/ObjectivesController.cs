using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestSystem.Data;
using TestSystem.Models;

public class ObjectivesController : Controller
{
    private readonly ApplicationDbContext _context;

    public ObjectivesController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [Authorize(Roles = "Admin,TaskCreator")]
    public IActionResult Create()
    {
        var model = new CreateObjectiveViewModel();
        model.Tests.Add(new TestCaseViewModel()); 
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,TaskCreator")]
    public async Task<IActionResult> Create(CreateObjectiveViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        
        var objective = new Objective
        {
            Title = model.Title,
            Description = model.Description,
            Tests = model.Tests.Select(t => new TestCase
            {
                Input = t.Input,
                ExpectedOutput = t.ExpectedOutput,
                IsHidden = t.IsHidden
            }).ToList()
        };

        _context.Objectives.Add(objective);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(List));
    }
    
    public IActionResult List()
    {
        var tasks = _context.Objectives.ToList();
        return View(tasks);
    }

    public IActionResult Details(int id)
    {
        var obj = _context.Objectives
            .Include(o => o.Tests)
            .FirstOrDefault(o => o.Id == id);

        if (obj == null) return NotFound();

        return View(obj);
    }
}