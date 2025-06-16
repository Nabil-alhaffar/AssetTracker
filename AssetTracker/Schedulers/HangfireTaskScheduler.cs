using AssetTracker.Services;
using Hangfire;

/// <summary>
/// Handles scheduling of recurring Hangfire background tasks.
/// </summary>
public class HangfireTaskScheduler
{
    private readonly ILogger<HangfireTaskScheduler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="HangfireTaskScheduler"/> class.
    /// </summary>
    /// <param name="logger">The logger instance used to log information.</param>
    public HangfireTaskScheduler(ILogger<HangfireTaskScheduler> logger)
    {
        _logger = logger;
    }


    /// <summary>
    /// Configures recurring Hangfire jobs for updating portfolio data and market value history.
    /// </summary>
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