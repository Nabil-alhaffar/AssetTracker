using AssetTracker.Services;
using Hangfire;

public class HangfireTaskScheduler
{
    private readonly ILogger<HangfireTaskScheduler> _logger;

    public HangfireTaskScheduler(ILogger<HangfireTaskScheduler> logger)
    {
        _logger = logger;
    }

    public void Configure()
    {
        _logger.LogInformation("Scheduling recurring tasks...");

        // Schedule the market value update at market close (8:00 PM UTC)
        RecurringJob.AddOrUpdate<PortfolioService>(
            "market-close-job", // Unique job ID
            service => service.UpdateTotalValuesForAllUsersAsync(),
            "0 20 * * *"); // Cron expression for 8:00 PM UTC
    }
}