namespace ClickView.Extensions.Hosting.Workers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Logging;

public abstract class CronWorker : Worker
{
    private readonly ILogger _logger;
    private readonly Random _delayGenerator = new();
    private readonly CronWorkerOption? _option;

    // Cron format: https://www.nuget.org/packages/Cronos/
    protected abstract string CronSchedule { get; }

    protected CronWorker(ILogger logger) : base(logger)
    {
        _logger = logger;
    }

    protected CronWorker(CronWorkerOption option, ILogger logger) : base(logger)
    {
        _option = option;
        _logger = logger;
    }

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

            var cronValue = CronExpression.Parse(CronSchedule, CronFormat.IncludeSeconds);
            var next = cronValue.GetNextOccurrence(now);
            if (!next.HasValue)
                return null;

            var delay = next.Value - now;

            if (_option is null || !_option.AllowExtraDelay)
                return delay;

            var extraDelay = TimeSpan.FromSeconds(_delayGenerator.Next((int)_option.MinDelayInSecond, (int)_option.MaxDelayInSecond));

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
