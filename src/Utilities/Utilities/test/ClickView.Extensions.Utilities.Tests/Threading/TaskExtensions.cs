namespace ClickView.Extensions.Utilities.Tests.Threading;

using System;
using System.Threading.Tasks;

internal static class TaskExtensions
{
    public static async Task IgnoreOperationCanceled(this Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
    }
}