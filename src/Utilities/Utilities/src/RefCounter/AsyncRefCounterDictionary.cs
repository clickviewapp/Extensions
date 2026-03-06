// ReSharper disable once CheckNamespace
namespace ClickView.Extensions.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class AsyncRefCounterDictionary<TKey, TVal>(Func<TKey, CancellationToken, Task<TVal>> createFunc)
    : IDisposable
    where TVal : IDisposable
    where TKey : notnull
{
    private readonly Dictionary<TKey, AsyncRefCounter<TVal>> _references = new();

    private readonly object _lock = new();

    public Task<RefCounterReference<TVal>> GetOrCreateAsync(TKey key)
    {
        return GetOrCreateAsync(key, CancellationToken.None);
    }

    public Task<RefCounterReference<TVal>> GetOrCreateAsync(TKey key, CancellationToken cancellationToken)
    {
        AsyncRefCounter<TVal>? refCounter;

        lock (_lock)
        {
            if (!_references.TryGetValue(key, out refCounter))
            {
                refCounter = new AsyncRefCounter<TVal>(ct => createFunc(key, ct), () => OnDisposed(key));
                _references.Add(key, refCounter);
            }
        }

        return refCounter.GetAsync(cancellationToken);
    }

    public void Remove(TKey key)
    {
        lock (_lock)
        {
            if (!_references.TryGetValue(key, out var value))
                return;

            //force dispose
            value.DisposeReference();

            _references.Remove(key);
        }
    }

    public bool HasReference(TKey key)
    {
        lock (_lock)
        {
            if (_references.TryGetValue(key, out var refCounter))
                return refCounter.HasReference();
        }

        return false;
    }

    private void OnDisposed(TKey key)
    {
        lock (_lock)
        {
            _references.Remove(key);
        }
    }

    public void Dispose()
    {
        List<AsyncRefCounter<TVal>> refs;

        lock (_lock)
        {
            refs = _references.Select(v => v.Value).ToList();
        }

        foreach(var o in refs)
        {
            o.DisposeReference();
        }
    }
}
