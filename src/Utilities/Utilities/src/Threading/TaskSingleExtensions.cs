namespace ClickView.Extensions.Utilities.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public static class TaskSingleExtensions
    {
        /// <summary>
        /// Executes the action returning the result. Only one action will run concurrently per <paramref name="key"/>
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TVal"></typeparam>
        /// <param name="instance"></param>
        /// <param name="key">A unique key which is used to identity the work to be executed</param>
        /// <param name="func">The work to execute asynchronously</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<TVal> RunAsync<TKey, TVal>(this TaskSingle<TKey> instance, TKey key,
            Func<TKey, CancellationToken, Task<TVal>> func, CancellationToken cancellationToken = default)
        {
            return instance.RunAsync(key, () => func(key, cancellationToken));
        }
    }
}
