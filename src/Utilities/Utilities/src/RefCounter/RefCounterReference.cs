// ReSharper disable once CheckNamespace
namespace ClickView.Extensions.Utilities
{
    using System;

    public sealed class RefCounterReference<T>(T value, Action onDispose) : IDisposable
        where T : IDisposable
    {
        public T Value { get; } = value;

        public void Dispose()
        {
            onDispose();
        }
    }
}
