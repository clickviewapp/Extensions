# ClickView.Extensions.Events


## Usage

### Registering Services

```cs
var services = new ServiceCollection();

services.AddEvents();
```

### Using a different event bus

If you don't wish to use the default In-Memory bus, you can specify a different bus via `IEventsBuilder`

```cs
var services = new ServiceCollection();

services.AddEvents()
    .UseBus<RedisBus>();
```



### Using the SimpleEventService

The `SimpleEventService` is a basic class which does basic handling and publishing of events. This can be used without having to worry about using Dependency Injection or building your own event service

```cs
var eventService = new SimpleEventService();

eventService.RegisterHandler<ExampleEvent>(evt => 
{
    Console.WriteLine("Hello!");
    return Task.CompletedTask;
});

await eventService.PublishAsync(new ExampleEvent());
```