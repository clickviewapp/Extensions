# ClickView.Extensions.Hosting

A basic console host which is similar to the AspNet host. Includes:
- DI
- Running as a Service/Console
- Logging
- Config

```cmd
install-package ClickView.Extensions.Hosting
```

# Console Host

### Example

```cs
private static async Task Main(string[] args)
{
    var isService = !(Debugger.IsAttached || args.Contains("--console"));

    var builder = ConsoleHost.CreateDefaultBuilder(args)
        .ConfigureServices((services =>
        {
        }));

    if (isService)
    {
        await builder.RunAsServiceAsync();
    }
    else
    {
        await builder.RunConsoleAsync();
    }
}
```


# Workers

Workers need to be registerd in DI as Hosted Services

```cs
services.AddHostedService<TimedPrinter>();
```

## Worker

Runs `ExecuteAsync` once.

### Example
```cs
public class ExampleWorker : Worker
{
    private readonly ILogger _logger;
    private readonly IQueueClient _queueClient;

    public ExampleWorker(ILogger logger, IQueueClient queueClient) : base(logger)
    {
        _logger = logger;
        _queueClient = queueClient;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.Register(() => _queueClient.Unsubscribe());

        return _queueClient.SubscribeAsync(OnEvent);
    }

    private Task OnEvent(Event evt)
    {
        _logger.LogInformation("New event! " + evt.Name);

        return Task.CompletedTask;
    }
}
```

## Interval Worker

Runs `LoopAsync` every time defined by `Interval`

### Example

```cs
public class TimedPrinter : IntervalWorker
{
    private readonly ILogger _logger;
    private int _count;

    public TimedPrinter(ILogger logger) : base(logger)
    {
        _logger = logger;
    }

    protected override TimeSpan Interval => TimeSpan.FromSeconds(1);

    protected override Task LoopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Hello! " + ++_count);

        return Task.CompletedTask;
    }
}
```
