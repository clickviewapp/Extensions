// ReSharper disable once CheckNamespace
namespace ClickView.Extensions.Utilities
{
    using System;

    public class RefCounterReference<T> : IDisposable where T : IDisposable
    {
        private readonly Action _onDispose;
        public T Value { get; }

        public RefCounterReference(T value, Action onDispose)
        {
            Value = value;
            _onDispose = onDispose;
        }

        public void Dispose()
        {
            _onDispose();
        }
    }
}