using System.ComponentModel.DataAnnotations;

namespace TestSystem.Models
{
    public class Objective
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public ICollection<Submission> Submissions { get; set; }
    }
}