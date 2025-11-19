using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestSystem.Services;
using TestSystem.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TestSystem.Models;

namespace TestSystem.Services
{
    public class SubmissionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<SubmissionBackgroundService> _logger;

        public SubmissionBackgroundService(IServiceProvider services, ILogger<SubmissionBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SubmissionBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();

                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var evaluator = scope.ServiceProvider.GetRequiredService<SubmissionEvaluator>();
                    
                    var submission = await db.Submissions
                        .Include(s => s.Objective)
                        .FirstOrDefaultAsync(s => s.Status == SubmissionStatus.Pending, stoppingToken);

                    if (submission != null)
                    {
                        _logger.LogInformation($"Found pending submission #{submission.Id}");
                        
                        await evaluator.EvaluateSubmissionAsync(submission.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background submission checking loop.");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
