namespace ClickView.Extensions.Hosting.Workers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public abstract class IntervalWorker : Worker
    {
        private readonly ILogger _logger;
        protected abstract TimeSpan Interval { get; }

        protected IntervalWorker(ILogger logger) : base(logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await LoopAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled Exception caught in IntervalWorker ({WorkerName})", Name);
                }

                try
                {
                    await Task.Delay(Interval, cancellationToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        protected abstract Task LoopAsync(CancellationToken cancellationToken);
    }
}
