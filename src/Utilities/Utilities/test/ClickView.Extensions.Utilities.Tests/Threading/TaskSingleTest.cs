﻿namespace ClickView.Extensions.Utilities.Tests.Threading
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Utilities.Threading;
    using Xunit;

    public class TaskSingleTest
    {
        [Fact]
        public async Task RunAsync_SameKey_MultipleCalls_RunOnce()
        {
            var instance = new TaskSingle<string>();

            var timesRun = 0;

            async Task<string> ThingToRunAsync(string key)
            {
                await Task.Delay(200);

                Interlocked.Increment(ref timesRun);

                return key;
            }

            var results = await Task.WhenAll(
                instance.RunAsync("test", (s, _) => ThingToRunAsync(s), CancellationToken.None),
                instance.RunAsync("test", (s, _) => ThingToRunAsync(s), CancellationToken.None),
                instance.RunAsync("test", (s, _) => ThingToRunAsync(s), CancellationToken.None)
            );

            Assert.Equal(1, timesRun);
            Assert.Single(results.Distinct());
        }

        [Fact]
        public async Task RunAsync_MultipleKeys_RunManyTimes()
        {
            var instance = new TaskSingle<string>();

            var timesRun = 0;

            async Task<string> ThingToRunAsync(string key)
            {
                await Task.Delay(200);

                Interlocked.Increment(ref timesRun);

                return key;
            }

            var results = await Task.WhenAll(
                instance.RunAsync("test1", (s, _) => ThingToRunAsync(s), CancellationToken.None),
                instance.RunAsync("test2", (s, _) => ThingToRunAsync(s), CancellationToken.None),
                instance.RunAsync("test3", (s, _) => ThingToRunAsync(s), CancellationToken.None)
            );

            Assert.Equal(3, timesRun);
            Assert.Equal(3, results.Distinct().Count());
        }

        [Fact]
        public async Task RunAsync_SameKey_NonParallel()
        {
            var instance = new TaskSingle<string>();

            var timesRun = 0;

            async Task<string> ThingToRunAsync(string key)
            {
                await Task.Delay(200);

                Interlocked.Increment(ref timesRun);

                return key;
            }

            await instance.RunAsync("test", (s, _) => ThingToRunAsync(s), CancellationToken.None);
            await instance.RunAsync("test", (s, _) => ThingToRunAsync(s), CancellationToken.None);

            Assert.Equal(2, timesRun);
        }
    }
}
