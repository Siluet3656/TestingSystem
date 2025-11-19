using System.ComponentModel.DataAnnotations;

public class EditObjectiveViewModel
{
    public int? Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Description { get; set; }

    public List<TestCaseViewModel> Tests { get; set; } = new();
}