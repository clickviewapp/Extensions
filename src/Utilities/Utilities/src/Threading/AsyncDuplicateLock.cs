namespace ClickView.Extensions.Utilities.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AsyncDuplicateLock
    {
        public static AsyncDuplicateLock<object> Default = new AsyncDuplicateLock<object>();
    }

    // Thanks StackOverflow!
    // http://stackoverflow.com/a/31194647/1757803
    public sealed class AsyncDuplicateLock<TKey>
    {
        private sealed class RefCounted<T>
        {
            public RefCounted(T value)
            {
                RefCount = 1;
                Value = value;
            }

            public int RefCount { get; set; }
            public T Value { get; }
        }

        private readonly Dictionary<TKey, RefCounted<SemaphoreSlim>> _semaphoreSlims = new Dictionary<TKey, RefCounted<SemaphoreSlim>>();

        private SemaphoreSlim GetOrCreate(TKey key)
        {
            RefCounted<SemaphoreSlim> item;
            lock (_semaphoreSlims)
            {
                if (_semaphoreSlims.TryGetValue(key, out item))
                {
                    ++item.RefCount;
                }
                else
                {
                    item = new RefCounted<SemaphoreSlim>(new SemaphoreSlim(1, 1));
                    _semaphoreSlims[key] = item;
                }
            }
            return item.Value;
        }

        public IDisposable Lock(TKey key)
        {
            return Lock(key, CancellationToken.None);
        }

        public IDisposable Lock(TKey key, CancellationToken cancellationToken)
        {
            var slim = GetOrCreate(key);

            try
            {
                slim.Wait(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                //We need to make sure we dispose/release the semaphore if the cancellation token is cancelled
                OnDisposed(key, true);
                throw;
            }

            return new Releaser(key, this);
        }

        public Task<IDisposable> LockAsync(TKey key)
        {
            return LockAsync(key, CancellationToken.None);
        }

        public async Task<IDisposable> LockAsync(TKey key, CancellationToken cancellationToken)
        {
            try
            {
                await GetOrCreate(key).WaitAsync(cancellationToken).ConfigureAwait(false);
                return new Releaser(key, this);
            }
            catch (OperationCanceledException)
            {
                OnDisposed(key, true);
                throw;
            }
        }

        private void OnDisposed(TKey key, bool cancelled)
        {
            RefCounted<SemaphoreSlim> item;
            lock (_semaphoreSlims)
            {
                item = _semaphoreSlims[key];
                --item.RefCount;
                if (item.RefCount == 0)
                    _semaphoreSlims.Remove(key);
            }

            // if the wait was cancelled, the semaphore has already been released
            if (!cancelled)
                item.Value.Release();
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncDuplicateLock<TKey> _locker;
            private readonly TKey _key;

            public Releaser(TKey key, AsyncDuplicateLock<TKey> locker)
            {
                _key = key;
                _locker = locker;
            }

            public void Dispose()
            {
                _locker.OnDisposed(_key, false);
            }
        }
    }
}
