﻿using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI;

internal class PageCollectionHelpers
{
    public static AsyncPageCollection<T> Create<T>(ClientToken firstPageToken, Func<ClientToken, RequestOptions?, Task<ClientPage<T>>> getPageAsync) where T : notnull
        => new FuncAsyncPageCollection<T>(firstPageToken, getPageAsync);

    public static PageCollection<T> Create<T>(ClientToken firstPageToken, Func<ClientToken, RequestOptions?, ClientPage<T>> getPage) where T : notnull
        => new FuncPageCollection<T>(firstPageToken, getPage);

    private class FuncAsyncPageCollection<T> : AsyncPageCollection<T> where T : notnull
    {
        private readonly ClientToken _firstPageToken;
        private readonly Func<ClientToken, RequestOptions?, Task<ClientPage<T>>> _getPageAsync;

        public FuncAsyncPageCollection(ClientToken firstPageToken, Func<ClientToken, RequestOptions?, Task<ClientPage<T>>> getPageAsync)
        {
            _firstPageToken = firstPageToken;
            _getPageAsync = getPageAsync;
        }

        public override ClientToken FirstPageToken => _firstPageToken;

        public override async Task<ClientPage<T>> GetPageAsync(ClientToken pageToken, RequestOptions? options = null)
            => await _getPageAsync(pageToken, options).ConfigureAwait(false);
    }

    private class FuncPageCollection<T> : PageCollection<T> where T : notnull
    {
        private readonly ClientToken _firstPageToken;
        private readonly Func<ClientToken, RequestOptions?, ClientPage<T>> _getPage;

        public FuncPageCollection(ClientToken firstPageToken, Func<ClientToken, RequestOptions?, ClientPage<T>> getPage)
        {
            _firstPageToken = firstPageToken;
            _getPage = getPage;
        }

        public override ClientToken FirstPageToken => _firstPageToken;

        public override ClientPage<T> GetPage(ClientToken pageToken, RequestOptions? options = null)
            => _getPage(pageToken, options);
    }
}
