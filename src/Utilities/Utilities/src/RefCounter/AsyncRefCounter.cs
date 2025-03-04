// ReSharper disable once CheckNamespace
namespace ClickView.Extensions.Utilities
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncRefCounter<T> where T : IDisposable
    {
        private readonly Action? _onDispose;
        private readonly Func<CancellationToken, Task<T>> _createFunc;
        private T? _reference;
        private bool _hasReference;
        private int _refCount;
        private readonly SemaphoreSlim _slimLock = new(1, 1);

        public AsyncRefCounter(Func<CancellationToken, Task<T>> createFunc)
        {
            _createFunc = createFunc;
        }

        public AsyncRefCounter(Func<CancellationToken, Task<T>> createFunc, Action onDispose) : this(createFunc)
        {
            _onDispose = onDispose;
        }

        public Task<RefCounterReference<T>> GetAsync()
        {
            return GetAsync(CancellationToken.None);
        }

        public async Task<RefCounterReference<T>> GetAsync(CancellationToken cancellationToken)
        {
            await _slimLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (!_hasReference)
                {
                    _reference = await _createFunc(cancellationToken).ConfigureAwait(false);
                    _hasReference = true;
                    _refCount = 0;
                }

                ++_refCount;
                return new RefCounterReference<T>(_reference!, OnDispose);
            }
            finally
            {
                _slimLock.Release();
            }
        }

        public bool HasReference()
        {
            _slimLock.Wait();

            try
            {
                return _hasReference;
            }
            finally
            {
                _slimLock.Release();
            }
        }

        /// <summary>
        /// Force a disposal of the underlying data
        /// </summary>
        public void DisposeReference()
        {
            _slimLock.Wait();

            try
            {
                DisposeReferenceInternal();
            }
            finally
            {
                _slimLock.Release();
            }

            _onDispose?.Invoke();
        }

        // RefCounterReference dispose callback
        private void OnDispose()
        {
            _slimLock.Wait();

            try
            {
                // Decrement refCount
                --_refCount;

                if (_refCount > 0)
                    return;

                DisposeReferenceInternal();
            }
            finally
            {
                _slimLock.Release();
            }
        }

        private void DisposeReferenceInternal()
        {
            if (!_hasReference)
            {
                // if no reference, reset refCount
                _refCount = 0;

                return;
            }

            _reference!.Dispose();

            _reference = default;
            _hasReference = false;
            _refCount = 0;
        }
    }
}
