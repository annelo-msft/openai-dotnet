﻿using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.VectorStores;

// Convenience version
public partial class VectorStoreFileBatchOperation : OperationResult
{
    // Convenience version
    internal VectorStoreFileBatchOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        ClientResult<VectorStoreBatchFileJob> result,
        RequestOptions options)
        : base(result.GetRawResponse())
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        _options = options;

        Value = result;
        Status = Value.Status;
        IsCompleted = GetIsCompleted(Value.Status);

        _vectorStoreId = Value.VectorStoreId;
        _batchId = Value.BatchId;

        _pollingInterval = new();

        RehydrationToken = new VectorStoreFileBatchOperationToken(VectorStoreId, BatchId);
    }

    // TODO: interesting question regarding whether these properties should be
    // nullable or not.  If someone has called the protocol method, do they want
    // to pay the perf cost of deserialization?  This could capitalize on a 
    // property on RequestOptions that allows the caller to opt-in to creation
    // of convenience models.  For now, make them nullable so I don't have to 
    // pass the model into the constructor from a protocol method.
    public VectorStoreBatchFileJob? Value { get; private set; }
    public VectorStoreBatchFileJobStatus? Status { get; private set; }

    public string VectorStoreId { get => _vectorStoreId; }
    public string BatchId { get => _batchId; }

    public override async Task WaitForCompletionAsync()
    {
        IAsyncEnumerator<ClientResult<VectorStoreBatchFileJob>> enumerator =
            new VectorStoreFileBatchOperationUpdateEnumerator(
                _pipeline, _endpoint, _vectorStoreId, _batchId, _options);

        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            ApplyUpdate(enumerator.Current);

            // TODO: Plumb through cancellation token
            await _pollingInterval.WaitAsync(_options.CancellationToken);
        }
    }

    public override void WaitForCompletion()
    {
        IEnumerator<ClientResult<VectorStoreBatchFileJob>> enumerator = 
            new VectorStoreFileBatchOperationUpdateEnumerator(
                _pipeline, _endpoint, _vectorStoreId, _batchId, _options);

        while (enumerator.MoveNext())
        {
            ApplyUpdate(enumerator.Current);

            _pollingInterval.Wait();
        }
    }

    private void ApplyUpdate(ClientResult<VectorStoreBatchFileJob> update)
    {
        Value = update;
        Status = Value.Status;

        IsCompleted = GetIsCompleted(Value.Status);
        SetRawResponse(update.GetRawResponse());
    }

    private static bool GetIsCompleted(VectorStoreBatchFileJobStatus status)
    {
        return status == VectorStoreBatchFileJobStatus.Completed ||
            status == VectorStoreBatchFileJobStatus.Cancelled ||
            status == VectorStoreBatchFileJobStatus.Failed;
    }

    // Generated convenience methods

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and job ID.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the ingestion operation. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> GetBatchFileJobAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await GetBatchFileJobAsync(_vectorStoreId, _batchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets an existing vector store batch file ingestion job from a known vector store ID and job ID.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A <see cref="VectorStoreBatchFileJob"/> instance representing the ingestion operation. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> GetBatchFileJob(CancellationToken cancellationToken = default)
    {
        ClientResult result = GetBatchFileJob(_vectorStoreId, _batchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual async Task<ClientResult<VectorStoreBatchFileJob>> CancelBatchFileJobAsync(CancellationToken cancellationToken = default)
    {
        ClientResult result = await CancelBatchFileJobAsync(_vectorStoreId, _batchId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Cancels an in-progress <see cref="VectorStoreBatchFileJob"/>.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> An updated <see cref="VectorStoreBatchFileJob"/> instance. </returns>
    public virtual ClientResult<VectorStoreBatchFileJob> CancelBatchFileJob(CancellationToken cancellationToken = default)
    {
        ClientResult result = CancelBatchFileJob(_vectorStoreId, _batchId, cancellationToken.ToRequestOptions());
        PipelineResponse response = result.GetRawResponse();
        VectorStoreBatchFileJob value = VectorStoreBatchFileJob.FromResponse(response);
        return ClientResult.FromValue(value, response);
    }

    /// <summary>
    /// Gets a page collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        VectorStoreFileAssociationCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        VectorStoreFileBatchesPageEnumerator enumerator = new(_pipeline, _endpoint,
            _vectorStoreId,
            _batchId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            options?.Filter?.ToString(),
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// Rehydrates a page collection of file associations from a page token.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="AsyncPageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="AsyncPageCollection{T}.GetAllValuesAsync(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="AsyncPageCollection{T}.GetCurrentPageAsync"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual AsyncPageCollection<VectorStoreFileAssociation> GetFileAssociationsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileBatchesPageToken pageToken = VectorStoreFileBatchesPageToken.FromToken(firstPageToken);

        if (_vectorStoreId != pageToken.VectorStoreId)
        {
            throw new ArgumentException(
                "Invalid page token. 'VectorStoreId' value does not match page token value.",
                nameof(firstPageToken));
        }

        if (_batchId != pageToken.BatchId)
        {
            throw new ArgumentException(
                "Invalid page token. 'BatchId' value does not match page token value.",
                nameof(firstPageToken));
        }

        VectorStoreFileBatchesPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.VectorStoreId,
            pageToken.BatchId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            pageToken.Filter,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// Gets a page collection of file associations associated with a vector store batch file job, representing the files
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="options"> Options describing the collection to return. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual PageCollection<VectorStoreFileAssociation> GetFileAssociations(
        VectorStoreFileAssociationCollectionOptions? options = default,
        CancellationToken cancellationToken = default)
    {
        VectorStoreFileBatchesPageEnumerator enumerator = new(_pipeline, _endpoint,
            _vectorStoreId,
            _batchId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            options?.Filter?.ToString(),
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// Rehydrates a page collection of file associations from a page token.
    /// that were scheduled for ingestion into the vector store.
    /// </summary>
    /// <param name="firstPageToken"> Page token corresponding to the first page of the collection to rehydrate. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <remarks> <see cref="PageCollection{T}"/> holds pages of values. To obtain a collection of values, call
    /// <see cref="PageCollection{T}.GetAllValues(System.Threading.CancellationToken)"/>. To obtain the current
    /// page of values, call <see cref="PageCollection{T}.GetCurrentPage"/>.</remarks>
    /// <returns> A collection of pages of <see cref="VectorStoreFileAssociation"/>. </returns>
    public virtual PageCollection<VectorStoreFileAssociation> GetFileAssociations(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        VectorStoreFileBatchesPageToken pageToken = VectorStoreFileBatchesPageToken.FromToken(firstPageToken);

        if (_vectorStoreId != pageToken.VectorStoreId)
        {
            throw new ArgumentException(
                "Invalid page token. 'VectorStoreId' value does not match page token value.",
                nameof(firstPageToken));
        }

        if (_batchId != pageToken.BatchId)
        {
            throw new ArgumentException(
                "Invalid page token. 'BatchId' value does not match page token value.",
                nameof(firstPageToken));
        }

        VectorStoreFileBatchesPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.VectorStoreId,
            pageToken.BatchId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            pageToken.Filter,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }
}