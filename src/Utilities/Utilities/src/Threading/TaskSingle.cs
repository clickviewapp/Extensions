namespace ClickView.Extensions.Utilities.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public sealed class TaskSingle
    {
        /// <summary>
        /// A default instance of TaskSingle using String as the key type
        /// </summary>
        public static readonly TaskSingle<string> Default = new();
    }

    /// <summary>
    /// A helper class which provides duplicate function call suppression
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class TaskSingle<TKey> where TKey : notnull
    {
        private readonly Dictionary<TKey, object> _completionTasks = new();

        /// <summary>
        /// Executes the action returning the result. Only one action will run concurrently per <paramref name="key"/>
        /// </summary>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="key">A unique key which is used to identity the work to be executed</param>
        /// <param name="func">The work to execute asynchronously</param>
        /// <returns></returns>
        public Task<TVal> RunAsync<TVal>(TKey key, Func<Task<TVal>> func)
        {
            lock (_completionTasks)
            {
                if (_completionTasks.TryGetValue(key, out var value))
                {
                    var tcs = (TaskCompletionSource<TVal>)value;

                    return tcs.Task;
                }
                else
                {
                    var tcs = new TaskCompletionSource<TVal>();

                    // Add to our internal list of completion tasks
                    _completionTasks.Add(key, tcs);

                    // Start the task
                    _ = RunAsync(key, tcs, func);

                    return tcs.Task;
                }
            }
        }

        private async Task RunAsync<TVal>(TKey key, TaskCompletionSource<TVal> tcs, Func<Task<TVal>> func)
        {
            TVal? result = default;
            Exception? throwException = null;

            try
            {
                result = await func().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throwException = ex;
            }

            lock (_completionTasks)
                _completionTasks.Remove(key);

            switch (throwException)
            {
                case null:
                    tcs.SetResult(result!);
                    break;
                case OperationCanceledException:
                    tcs.SetCanceled();
                    break;
                default:
                    tcs.SetException(throwException);
                    break;
            }
        }
    }
}
