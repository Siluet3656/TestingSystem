using Microsoft.EntityFrameworkCore;
using TestSystem.Data;
using TestSystem.Models;

namespace TestSystem.Services
{
    public class SubmissionEvaluator
    {
        private readonly ApplicationDbContext _context;
        private readonly CodeRunner _codeRunner;
        private readonly ILogger<SubmissionEvaluator> _logger;

        public SubmissionEvaluator(ApplicationDbContext context, CodeRunner codeRunner, ILogger<SubmissionEvaluator> logger)
        {
            _context = context;
            _codeRunner = codeRunner;
            _logger = logger;
        }

        public async Task EvaluateAsync(int submissionId)
        {
            var submission = await _context.Submissions.FindAsync(submissionId);
            if (submission == null)
            {
                _logger.LogWarning("Submission {Id} not found.", submissionId);
                return;
            }

            try
            {
                _logger.LogInformation("Evaluating submission {Id}...", submissionId);

                var result = await _codeRunner.RunAsync(submission.Code, submission.Language);

                if (result.Status == "CompilationError")
                {
                    submission.Status = SubmissionStatus.CompilationError;
                }
                else if (result.ExitCode != 0)
                {
                    submission.Status = SubmissionStatus.RuntimeError;
                }
                else
                {
                    submission.Status = SubmissionStatus.Accepted;
                }

                submission.PassedTests = submission.Status == SubmissionStatus.Accepted ? 1 : 0;
                submission.TotalTests = 1;
                submission.Output = result.Output ?? "";
                submission.ErrorMessage = result.ErrorMessage ?? "";

                _logger.LogInformation("Submission {Id} evaluated. Status={Status}", submissionId, submission.Status);
            }
            catch (Exception ex)
            {
                submission.Status = SubmissionStatus.RuntimeError;
                submission.ErrorMessage = ex.Message;
                _logger.LogError(ex, "Error evaluating submission {Id}", submissionId);
            }

            await _context.SaveChangesAsync();
        }
    }
}
