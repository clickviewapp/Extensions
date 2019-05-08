namespace ClickView.Extensions.RestClient.Authenticators.OAuth.Internal
{
    using System;
    using System.Threading.Tasks;

    internal class AsyncLazy<T> : Lazy<Task<T>>
    {
        public AsyncLazy(Func<Task<T>> taskFactory) : base(() => Task.Factory.StartNew(taskFactory).Unwrap())
        {
        }
    }
}
