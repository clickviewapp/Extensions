namespace ClickView.Extensions.Hosting.Tests;

using Exceptions;
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

        Assert.True(scheduler.Counter >= 2);

        scheduler.Dispose();
    }

    [Fact]
    public void SchedulerOption_MinGreaterThanMax_ThrowException()
    {
        Assert.Throws<InvalidSchedulerOptionException>(() => 
            { new SchedulerOption(true, 2, 1); });
    }

    [Fact]
    public async Task RunAsync_RunEveryTwoSeconds_return()
    {
        var everyTwoSecondCron = "*/2 * * * * *";
        var option = new SchedulerOption(true, 2, 3);

        var mockLogger = new Mock<ILogger>();
        var scheduler = new TestSchedulerWithOption(everyTwoSecondCron, option, mockLogger.Object);

        await scheduler.StartAsync(CancellationToken.None);

        var currentTime = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(4));

        await scheduler.StopAsync(CancellationToken.None);

        Assert.NotNull(scheduler.FirstExecutionTime);
        Assert.True(scheduler.FirstExecutionTime.Value - currentTime > TimeSpan.FromSeconds(2));

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

    public class TestSchedulerWithOption : SchedulerWorker
    {
        public DateTime? FirstExecutionTime { get; set; }

        public TestSchedulerWithOption(string cron, SchedulerOption option, ILogger logger) : base(option, logger)
        {
            CronSchedule = cron;
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            FirstExecutionTime ??= DateTime.UtcNow;

            return Task.CompletedTask;
        }

        protected override string CronSchedule { get; }
    }
}