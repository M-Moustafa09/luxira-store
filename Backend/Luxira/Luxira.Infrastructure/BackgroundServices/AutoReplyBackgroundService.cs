using Luxira.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Luxira.Infrastructure.BackgroundServices;

// First background job in the project (Module B). Polls every ~30s for
// reviews whose AutoReplyDueAt has passed and creates the auto-reply - a
// plain persisted due-time + polling BackgroundService rather than an
// in-memory timer (survives app restarts, since AutoReplyDueAt/AutoReplySent
// live on the Review row itself) or a job library like Hangfire/Quartz.NET
// (would add a new dependency and its own tables for scheduling exactly one
// kind of one-shot job).
//
// Registered as a singleton (like any IHostedService), so it resolves a new
// DI scope per poll to get scoped services - the actual per-cycle logic
// lives in IAutoReplyProcessor (a normal scoped Service class), keeping this
// class a thin scheduling wrapper.
public class AutoReplyBackgroundService : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AutoReplyBackgroundService> _logger;

    public AutoReplyBackgroundService(IServiceScopeFactory scopeFactory, ILogger<AutoReplyBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PollInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IAutoReplyProcessor>();
                await processor.ProcessDueRepliesAsync();
            }
            catch (Exception ex)
            {
                // A single failed poll cycle must not kill the loop - the
                // next tick will simply retry the (still-due) rows.
                _logger.LogError(ex, "Failed to process due auto-replies.");
            }
        }
    }
}
