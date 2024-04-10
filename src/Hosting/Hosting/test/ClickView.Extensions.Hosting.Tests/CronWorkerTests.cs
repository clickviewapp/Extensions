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
        var everyTwoSecondCron = "*/2 * * * * *";

        var mockLogger = new Mock<ILogger>();
        var scheduler = new TestScheduler(everyTwoSecondCron, mockLogger.Object);

        await scheduler.StartAsync(CancellationToken.None);

        await Task.Delay(TimeSpan.FromSeconds(5));

        await scheduler.StopAsync(CancellationToken.None);

        Assert.True(scheduler.Counter >= 2);
    }

    [Fact]
    public void CronWorkerOption_MinGreaterThanMax_ThrowException()
    {
        Assert.Throws<InvalidSchedulerOptionException>(() => 
            { new CronWorkerOption(true, 2, 1); });
    }

    [Fact]
    public async Task RunAsync_RunEveryTwoSeconds_return()
    {
        var everyTwoSecondCron = "*/2 * * * * *";
        var option = new CronWorkerOption(true, 2, 3);

        var mockLogger = new Mock<ILogger>();
        var scheduler = new TestSchedulerWithOption(everyTwoSecondCron, option, mockLogger.Object);

        await scheduler.StartAsync(CancellationToken.None);

        var currentTime = DateTime.UtcNow;

        await Task.Delay(TimeSpan.FromSeconds(4));

        await scheduler.StopAsync(CancellationToken.None);

        Assert.NotNull(scheduler.FirstExecutionTime);
        Assert.True(scheduler.FirstExecutionTime.Value - currentTime > TimeSpan.FromSeconds(2));
    }

    public class TestScheduler : CronWorker
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

    public class TestSchedulerWithOption : CronWorker
    {
        public DateTime? FirstExecutionTime { get; set; }

        public TestSchedulerWithOption(string cron, CronWorkerOption option, ILogger logger) : base(option, logger)
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