using Microsoft.Extensions.Logging;
using TestSystem.Data;
using TestSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TestSystem.Services
{
    public class SubmissionEvaluator
    {
        private readonly ApplicationDbContext _db;
        private readonly CodeRunner _runner;
        private readonly ILogger<SubmissionEvaluator> _logger;

        public SubmissionEvaluator(
            ApplicationDbContext db,
            CodeRunner runner,
            ILogger<SubmissionEvaluator> logger)
        {
            _db = db;
            _runner = runner;
            _logger = logger;
        }

        public async Task EvaluateSubmissionAsync(int submissionId)
        {
            _logger.LogInformation("Start evaluating submission #{id}", submissionId);
            
            var submission = await _db.Submissions
                .Include(s => s.Objective)
                .ThenInclude(o => o.Tests) 
                .FirstOrDefaultAsync(s => s.Id == submissionId);

            if (submission == null)
            {
                _logger.LogError("Submission {id} not found!", submissionId);
                return;
            }

            submission.Status = SubmissionStatus.Running;
            await _db.SaveChangesAsync();

            CompilationResult compilation = null;

            try
            {
                compilation = await _runner.CompileAsync(submission.Code, submission.Language);
                
                if (!compilation.Success)
                {
                    submission.Status = SubmissionStatus.CompilationError;
                    submission.ErrorMessage = compilation.Error;
                    await _db.SaveChangesAsync();
                    return;
                }

                var tests = submission.Objective.Tests.ToList();

                submission.TotalTests = tests.Count;
                submission.PassedTests = 0;
                
                foreach (var test in tests)
                {
                    var result = await _runner.RunAsync(
                        compilation.ExecutablePath, 
                        compilation.WorkingDirectory, 
                        test.Input
                    );
                    
                    _logger.LogInformation("Program OUT: " + result.Output);
                    _logger.LogError("Program ERR: " + result.Error);

                    if (!string.IsNullOrEmpty(result.Error))
                    {
                        submission.Status = SubmissionStatus.RuntimeError;
                        submission.ErrorMessage = result.Error;
                        break;
                    }

                    if (result.Output.Trim() == test.ExpectedOutput.Trim())
                        submission.PassedTests++;
                }

                if (submission.PassedTests == submission.TotalTests)
                    submission.Status = SubmissionStatus.Accepted;
                else if (submission.Status != SubmissionStatus.RuntimeError)
                    submission.Status = SubmissionStatus.WrongAnswer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Evaluation failed");
                submission.Status = SubmissionStatus.RuntimeError;
                submission.ErrorMessage = ex.Message;
            }
            finally
            {
                await _db.SaveChangesAsync();
            }

            _logger.LogInformation(
                "Finished submission #{id} with status {status}",
                submission.Id, submission.Status);
        }
    }
}