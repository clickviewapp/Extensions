namespace ClickView.Extensions.Hosting.Workers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Logging;

public abstract class CronWorker(CronWorkerOption option, ILogger logger) : Worker(logger)
{
    private readonly ILogger _logger = logger;
    private readonly Random _delayGenerator = new();
    private readonly CronWorkerOption? _option = option;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var dueTime = GetNextScheduleDelay();
            if (!dueTime.HasValue)
            {
                _logger.LogWarning("Failed to get next schedule");

                // Wait for a while before retrying
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                continue;
            }

            try
            {
                await Task.Delay(dueTime.Value, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                await RunAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught in CronWorker ({WorkerName})", Name);
            }
        }
    }

    private TimeSpan? GetNextScheduleDelay()
    {
        // Use try/catch here so that we don't crash the app if anything goes wrong with getting the next schedule time
        try
        {
            var now = DateTime.UtcNow;

            var cronValue = CronExpression.Parse(_option?.Schedule, CronFormat.IncludeSeconds);
            var next = cronValue.GetNextOccurrence(now);
            if (!next.HasValue)
                return null;

            var delay = next.Value - now;

            if (_option is null || !_option.AllowJitter)
                return delay;

            var extraDelay = TimeSpan.FromSeconds(_delayGenerator.Next((int)_option.MinJitter.TotalSeconds, (int)_option.MaxJitter.TotalSeconds));

            return delay.Add(extraDelay);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get the next schedule time");
        }

        return null;
    }

    protected abstract Task RunAsync(CancellationToken cancellationToken);
}
