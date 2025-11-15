using System;
using System.ComponentModel.DataAnnotations;

namespace TestSystem.Models
{
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public int TaskId { get; set; } = 1; // TEST

        [Required]
        public string Code { get; set; }

        [Required]
        public string Language { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}