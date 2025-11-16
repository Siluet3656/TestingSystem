using System.ComponentModel.DataAnnotations;

namespace TestSystem.Models
{
    public class Objective
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        
        public ICollection<Submission> Submissions { get; set; }
        
        public ICollection<TestCase> Tests { get; set; }
    }
}