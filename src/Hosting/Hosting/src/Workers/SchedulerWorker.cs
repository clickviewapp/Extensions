namespace ClickView.Extensions.Hosting.Workers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Cronos;
using Microsoft.Extensions.Logging;

public abstract class SchedulerWorker : Worker, IDisposable
{
    private readonly ILogger _logger;
    private readonly Random _delayGenerator = new();
    private readonly SchedulerOption? _option;
    private Timer? _timer;
    private CancellationTokenSource? _cancellationTokenSource;

    // Cron format: https://www.nuget.org/packages/Cronos/
    protected abstract string CronSchedule { get; }

    protected SchedulerWorker(ILogger logger) : base(logger)
    {
        _logger = logger;
    }

    protected SchedulerWorker(SchedulerOption option, ILogger logger) : base(logger)
    {
        _option = option;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var dueTime = GetNextScheduleDelay();
        if (!dueTime.HasValue)
            throw new Exception("Failed to get next schedule");

        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _timer = new Timer(_ => TimerCallbackAsync(_cancellationTokenSource.Token), null, dueTime.Value, Timeout.InfiniteTimeSpan);

        return Task.CompletedTask;
    }

    private async void TimerCallbackAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;

        try
        {
            await RunAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught in SchedulerWorker ({WorkerName})", Name);
        }
        finally
        {
            var dueTime = GetNextScheduleDelay();
            if (dueTime.HasValue)
            {
                // Reschedule the timer
                _timer?.Change(dueTime.Value, Timeout.InfiniteTimeSpan);
            }
            else
            {
                _logger.LogError("Unable to get the next schedule in SchedulerWorker ({WorkerName})", Name);
            }
        }
    }

    private TimeSpan? GetNextScheduleDelay()
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

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();

        // Stop the timer
        _timer?.Change(Timeout.Infinite, 0);
        _timer?.Dispose();
    }

    protected abstract Task RunAsync(CancellationToken cancellationToken);
}
