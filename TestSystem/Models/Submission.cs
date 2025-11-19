using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TestSystem.Models
{
    public enum SubmissionStatus
    {
        Pending = 0,
        Accepted = 1,
        WrongAnswer = 2,
        CompilationError = 3,
        RuntimeError = 4,
        Running = 5
    }
    
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }

        [Required]
        public int ObjectiveId { get; set; }

        [ForeignKey("ObjectiveId")]
        public Objective Objective { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Language { get; set; }

        public SubmissionStatus Status { get; set; }

        public int PassedTests { get; set; }

        public int TotalTests { get; set; }

        public string Output { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}