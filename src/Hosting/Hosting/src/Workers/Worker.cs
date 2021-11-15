namespace ClickView.Extensions.Hosting.Workers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public abstract class Worker : IHostedService
    {
        private readonly ILogger _logger;
        private Task? _executingTask;
        private CancellationTokenSource? _cts;

        protected Worker(ILogger logger)
        {
            _logger = logger;
        }

        protected virtual string Name => GetType().Name;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Worker {WorkerName}", Name);

            // Create a linked token so we can trigger cancellation outside of this token's cancellation
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Store the task we're executing
            _executingTask = ExecuteAsync(_cts.Token);

            _logger.LogInformation("Worker {WorkerName} started", Name);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
                return _executingTask;

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_executingTask == null)
                return;

            _logger.LogInformation("Stopping Worker {WorkerName}...", Name);

            // Signal cancellation to the executing method
            _cts?.Cancel();

            // Wait until the task completes or the stop token triggers
            await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken)).ConfigureAwait(false);

            _logger.LogInformation("Worker {WorkerName} stopped", Name);

            // Dispose our CancellationTokenSource and cleanup
            _cts?.Dispose();
            _cts = null;
            _executingTask = null;

            // Throw if cancellation triggered
            cancellationToken.ThrowIfCancellationRequested();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
