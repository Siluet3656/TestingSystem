using System.ComponentModel.DataAnnotations;
using TestSystem.Models;

public class TestCase
{
    public int Id { get; set; }

    [Required]
    public string Input { get; set; }

    [Required]
    public string ExpectedOutput { get; set; }
    
    public bool IsHidden { get; set; }

    public int ObjectiveId { get; set; }
    public Objective Objective { get; set; }
}