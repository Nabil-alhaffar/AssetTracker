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
        // Schedule the market value update thirty mins after market close (8:30 PM UTC)
        // Step 1: Update portfolio first at 8:30 AM and 8:30 PM UTC
        RecurringJob.AddOrUpdate<PortfolioService>(
            "twice-daily-portfolio-update",
            service => service.UpdatePortfolioForAllUsersAsync(),
            "30 8,20 * * *"); // Runs daily at 8:30 AM & 8:30 PM UTC

        // Step 2: Update total values AFTER the evening portfolio update (8:35 PM UTC)
        RecurringJob.AddOrUpdate<PortfolioService>(
            "market-close-job",
            service => service.UpdateTotalValuesForAllUsersAsync(),
            "35 20 * * *"); // Runs daily at 8:35 PM UTC
    }
}