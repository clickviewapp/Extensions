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
        public static TaskSingle<string> Default = new TaskSingle<string>();
    }

    /// <summary>
    /// A helper class which provides duplicate function call suppression
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class TaskSingle<TKey>
    {
        private readonly Dictionary<TKey, object> _completionTasks = new Dictionary<TKey, object>();

        /// <summary>
        /// Executes the action returning the result. Only one action will run concurrently per <param name="key">key</param>
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

                    Task.Run(() => { });

                    return tcs.Task;
                }
                else
                {
                    var tcs = new TaskCompletionSource<TVal>();

                    _ = Runner(key, tcs, func);

                    _completionTasks.Add(key, tcs);

                    return tcs.Task;
                }
            }
        }

        private async Task Runner<TVal>(TKey key, TaskCompletionSource<TVal> tcs, Func<Task<TVal>> func)
        {
            try
            {
                var result = await func();

                tcs.SetResult(result);
            }
            catch (OperationCanceledException)
            {
                tcs.SetCanceled();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            lock (_completionTasks)
            {
                _completionTasks.Remove(key);
            }
        }
    }
}