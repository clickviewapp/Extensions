namespace ClickView.Extensions.Hosting.Tests;

using Microsoft.Extensions.Logging;
using Moq;
using Workers;

public class SchedulerWorkerTests
{
    [Fact]
    public async Task RunAsync_RunEveryTwoSeconds()
    {
        var everyTwoSecondCron = "*/2 * * * * *";

        var mockLogger = new Mock<ILogger>();
        var scheduler = new TestScheduler(everyTwoSecondCron, mockLogger.Object);

        await scheduler.StartAsync(CancellationToken.None);

        await Task.Delay(TimeSpan.FromSeconds(5));

        await scheduler.StopAsync(CancellationToken.None);

        Assert.InRange(scheduler.Counter, 2, 3);

        scheduler.Dispose();
    }

    public class TestScheduler : SchedulerWorker
    {
        public int Counter { get; set; }

        public TestScheduler(string cron, ILogger logger) : base(logger)
        {
            CronSchedule = cron;
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            Counter++;
            return Task.CompletedTask;
        }

        protected override string CronSchedule { get; }
    }
}