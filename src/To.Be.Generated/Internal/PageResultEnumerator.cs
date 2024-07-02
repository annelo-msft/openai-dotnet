﻿using System;
using System.ClientModel;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

// Note: implements both sync and async enumerator interfaces.
internal abstract class PageResultEnumerator : IAsyncEnumerator<ClientResult>, IEnumerator<ClientResult>
{
    private ClientResult? _current;
    private bool _hasNext = true;

    public ClientResult Current => _current!;

    #region Service-specific methods that need to be generated on the subclient

    public abstract Task<ClientResult> GetFirstAsync();

    public abstract ClientResult GetFirst();

    public abstract Task<ClientResult> GetNextAsync(ClientResult result);

    public abstract ClientResult GetNext(ClientResult result);

    public abstract bool HasNext(ClientResult result);

    #endregion

    #region IEnumerator<ClientResult> implementation

    object IEnumerator.Current => ((IEnumerator<ClientResult>)this).Current;

    public bool MoveNext()
    {
        if (!_hasNext)
        {
            return false;
        }

        if (_current == null)
        {
            _current = GetFirst();
        }
        else
        {
            _current = GetNext(_current);
        }

        _hasNext = HasNext(_current);
        return true;
    }

    void IEnumerator.Reset() => _current = null;

    void IDisposable.Dispose() { }

    #endregion

    #region IAsyncEnumerator<ClientResult> implementation

    public async ValueTask<bool> MoveNextAsync()
    {
        if (!_hasNext)
        {
            return false;
        }

        if (_current == null)
        {
            _current = await GetFirstAsync().ConfigureAwait(false);
        }
        else
        {
            _current = await GetNextAsync(_current).ConfigureAwait(false);
        }

        _hasNext = HasNext(_current);
        return true;
    }

    ValueTask IAsyncDisposable.DisposeAsync() => default;

    #endregion
}