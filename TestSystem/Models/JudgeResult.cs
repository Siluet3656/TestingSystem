using TestSystem.Models;

namespace TestSystem.Models
{
    public class JudgeResult
    {
        public SubmissionStatus Status { get; set; }
        public int Passed { get; set; }
        public int TotalTests { get; set; }
        public string Output { get; set; }
        public string ErrorMessage { get; set; }
    }
}