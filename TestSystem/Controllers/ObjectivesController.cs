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
    
    [Authorize(Roles = "Admin,TaskCreator")]
    public async Task<IActionResult> Edit(int id)
    {
        var obj = await _context.Objectives
            .Include(o => o.Tests)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (obj == null)
            return NotFound();

        var vm = new EditObjectiveViewModel
        {
            Id = obj.Id,
            Title = obj.Title,
            Description = obj.Description,
            Tests = obj.Tests.Select(t => new TestCaseViewModel
            {
                Id = t.Id,
                Input = t.Input,
                ExpectedOutput = t.ExpectedOutput,
                IsHidden = t.IsHidden
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,TaskCreator")]
    public async Task<IActionResult> Edit(EditObjectiveViewModel model, [FromServices] ILogger<ObjectivesController> logger)
    {
        logger.LogInformation("Edit POST called for ObjectiveId={Id}", model.Id);

        foreach (var t in model.Tests)
        {
            logger.LogInformation("Test Id={Id}, Input='{Input}', Output='{Output}', IsHidden={IsHidden}, ToDelete={ToDelete}",
                t.Id, t.Input, t.ExpectedOutput, t.IsHidden, t.ToDelete);
        }

        ModelState.Clear();
        if (!TryValidateModel(model))
        {
            logger.LogWarning("Model validation failed.");
            return View(model);
        }

        var obj = await _context.Objectives
            .Include(o => o.Tests)
            .FirstOrDefaultAsync(o => o.Id == model.Id);

        if (obj == null) return NotFound();

        obj.Title = model.Title;
        obj.Description = model.Description;

        foreach (var testVm in model.Tests)
        {
            if (testVm.ToDelete && testVm.Id != null)
            {
                var testToDelete = obj.Tests.First(t => t.Id == testVm.Id);
                _context.TestCases.Remove(testToDelete);
                logger.LogInformation("Removed test {Id}", testVm.Id);
                continue;
            }

            if (testVm.Id == null)
            {
                obj.Tests.Add(new TestCase
                {
                    Input = testVm.Input,
                    ExpectedOutput = testVm.ExpectedOutput,
                    IsHidden = testVm.IsHidden
                });
                logger.LogInformation("Added new test with input '{Input}'", testVm.Input);
            }
            else
            {
                var existing = obj.Tests.First(t => t.Id == testVm.Id);
                existing.Input = testVm.Input;
                existing.ExpectedOutput = testVm.ExpectedOutput;
                existing.IsHidden = testVm.IsHidden;
                logger.LogInformation("Updated test {Id}", existing.Id);
            }
        }

        await _context.SaveChangesAsync();
        logger.LogInformation("Objective {Id} saved successfully", obj.Id);

        return RedirectToAction("Details", new { id = obj.Id });
    }
}