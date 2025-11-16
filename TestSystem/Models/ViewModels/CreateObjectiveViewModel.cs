using System.ComponentModel.DataAnnotations;

public class CreateObjectiveViewModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    public List<TestCaseViewModel> Tests { get; set; } = new();
}