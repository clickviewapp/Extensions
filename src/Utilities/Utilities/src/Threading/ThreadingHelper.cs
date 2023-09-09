namespace ClickView.Extensions.Utilities.Threading;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Primitives.Extensions;

public static class ThreadingHelper
{
    public static async Task<TOut[]> ProcessAsync<TIn, TOut>(IEnumerable<TIn> source,
        Func<TIn, CancellationToken, Task<TOut>> func,
        int concurrentTasks = 1,
        CancellationToken cancellationToken = default)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));

        if (func is null)
            throw new ArgumentNullException(nameof(func));

        var items = source.AsReadOnlyList();

        // If we have no items then theres nothing to do
        if (items.Count == 0)
            return Array.Empty<TOut>();

        // If our concurrentTasks is 1, we can just run each task one by one and avoid a lot of allocations
        if (concurrentTasks == 1)
            return await ProcessSerialAsync(items, func, cancellationToken);

        // Check if the count is less than the maxDegrees
        //If it is then we can just run all tasks at once without the need for a SemaphoreSlim
        if (items.Count <= concurrentTasks)
            return await ProcessParallelAsync(items, func, cancellationToken);

        using var semaphore = new SemaphoreSlim(concurrentTasks, concurrentTasks);
        var tasks = new Task<TOut>[items.Count];

        for (var i = 0; i < items.Count; i++)
        {
            await semaphore.WaitAsync(cancellationToken);

            tasks[i] = ProcessItemAsync(items[i], func, semaphore, cancellationToken);
        }

        return await Task.WhenAll(tasks);

        static async Task<TOut> ProcessItemAsync(TIn item,
            Func<TIn, CancellationToken, Task<TOut>> func,
            SemaphoreSlim semaphoreSlim,
            CancellationToken cancellationToken)
        {
            try
            {
                return await func(item, cancellationToken);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }

    private static async Task<TOut[]> ProcessSerialAsync<TIn, TOut>(IReadOnlyList<TIn> objs,
        Func<TIn, CancellationToken, Task<TOut>> func, CancellationToken cancellationToken = default)
    {
        var results = new TOut[objs.Count];

        for (var i = 0; i < objs.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            results[i] = await func(objs[i], cancellationToken);
        }

        return results;
    }

    private static async Task<TOut[]> ProcessParallelAsync<TIn, TOut>(IReadOnlyList<TIn> objs,
        Func<TIn, CancellationToken, Task<TOut>> func, CancellationToken cancellationToken = default)
    {
        switch (objs.Count)
        {
            case 0:
                return Array.Empty<TOut>();
            case 1:
                return new[] { await func(objs[0], cancellationToken) };
            default:
            {
                var taskList = new Task<TOut>[objs.Count];
                for (var i = 0; i < objs.Count; i++)
                {
                    taskList[i] = func(objs[i], cancellationToken);
                }

                return await Task.WhenAll(taskList);
            }
        }
    }
}
