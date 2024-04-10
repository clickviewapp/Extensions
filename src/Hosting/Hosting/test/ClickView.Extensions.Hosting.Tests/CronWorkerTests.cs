namespace ClickView.Extensions.Hosting.Tests;

using Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Workers;

public class CronWorkerTests
{
    [Fact]
    public async Task RunAsync_RunEveryTwoSeconds()
    {
        const string everyTwoSecondCron = "*/2 * * * * *";
        var option = new CronWorkerOption(everyTwoSecondCron);

        var mockLogger = new Mock<ILogger>();
        var scheduler = new TestScheduler(option, mockLogger.Object);

        await scheduler.StartAsync(CancellationToken.None);

        await Task.Delay(TimeSpan.FromSeconds(5));

        await scheduler.StopAsync(CancellationToken.None);

        Assert.True(scheduler.Counter >= 2);
    }

    [Fact]
    public void CronWorkerOption_MinGreaterThanMax_ThrowException()
    {
        const string everyTwoSecondCron = "*/2 * * * * *";

        Assert.Throws<InvalidCronWorkerOptionException>(() => 
            { new CronWorkerOption(everyTwoSecondCron, true, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1)); });
    }

    [Fact]
    public async Task RunAsync_RunEveryTwoSecondsWithJitter_Executes()
    {
        const string everyTwoSecondCron = "*/2 * * * * *";
        var option = new CronWorkerOption(everyTwoSecondCron, true, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3));

        var mockLogger = new Mock<ILogger>();
        var scheduler = new TestScheduler(option, mockLogger.Object);

        await scheduler.StartAsync(CancellationToken.None);

        var currentTime = DateTime.UtcNow;

        // Give it a bit more time to ensure the task is executed
        await Task.Delay(TimeSpan.FromSeconds(4));

        await scheduler.StopAsync(CancellationToken.None);

        Assert.NotNull(scheduler.FirstExecutionTime);
        Assert.True(scheduler.FirstExecutionTime.Value - currentTime > TimeSpan.FromSeconds(2));
    }

    public class TestScheduler(CronWorkerOption option, ILogger logger) : CronWorker(option, logger)
    {
        public int Counter { get; set; }
        public DateTime? FirstExecutionTime { get; set; }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            FirstExecutionTime ??= DateTime.UtcNow;

            Counter++;

            return Task.CompletedTask;
        }
    }
}