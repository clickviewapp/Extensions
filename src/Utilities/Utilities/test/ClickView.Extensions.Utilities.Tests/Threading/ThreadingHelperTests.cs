namespace ClickView.Extensions.Utilities.Tests.Threading;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Threading;
using Xunit;

public class ThreadingHelperTests
{
    [Fact]
    public async Task ProcessAsync_MoreItemsThanConcurrentTasks_LimitedToConcurrentTasks()
    {
        const int concurrency = 2;
        var taskCount = 0;

        // Create a list of more than 'concurrency' items
        var items = Enumerable.Range(0, concurrency + 1);

        using var cancellation = new CancellationTokenSource();

        var task = ThreadingHelper.ProcessAsync(items, ProcessFunc, concurrency, cancellation.Token);

        // wait until both tasks have started
        await Task.Delay(10, CancellationToken.None);

        // Check only our expected task number of tasks are running
        Assert.Equal(concurrency, taskCount);

        // Cleanup
        cancellation.Cancel();
        await task.IgnoreOperationCanceled();

        return;

        async Task<int> ProcessFunc(int x, CancellationToken y)
        {
            Interlocked.Increment(ref taskCount);
            // 1000 delay to ensure we have a timeout
            await Task.Delay(1000, y);
            Interlocked.Decrement(ref taskCount);
            return x;
        }
    }

    [Fact]
    public async Task ProcessAsync_ProcessesAllTasks()
    {
        const int concurrency = 4;
        const int totalTasks = 20;
        var taskCount = 0;

        // Create a list of more than 2 items
        var items = Enumerable.Range(0, totalTasks);

        var taskCounts = await ThreadingHelper.ProcessAsync(items, ProcessFunc, concurrency, CancellationToken.None);

        // Check that we ran all tasks
        Assert.Equal(totalTasks, taskCounts.Length);

        // Check that we never had more than 'concurrency' tasks running
        Assert.Equal(concurrency, taskCounts.Max());

        return;

        async Task<int> ProcessFunc(int x, CancellationToken y)
        {
            // Increment the task count and keep a reference to the current number
            var currentTasks = Interlocked.Increment(ref taskCount);

            // Simulate work
            await Task.Delay(10, y);

            // Reduce task count
            Interlocked.Decrement(ref taskCount);

            // Return the current task count we saw so we can check this value
            return currentTasks;
        }
    }
}
