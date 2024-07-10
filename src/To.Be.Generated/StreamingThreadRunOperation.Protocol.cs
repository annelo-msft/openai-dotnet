﻿using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.ServerSentEvents;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace OpenAI.Assistants;

// Protocol version
public partial class StreamingThreadRunOperation : OperationResult
{
    private readonly Uri _endpoint;
    
    private readonly string _threadId;

    // TODO: We have to set this from an event!
    private string _runId;

    // TODO: allocate differently based on delayed request or not per IDisposable
    private IAsyncEnumerable<SseItem<byte[]>>? _asyncEventStream;
    //private IEnumerable<SseItem<byte[]>>? _eventStream;

    // Note: what does it mean to have a protocol type for status?
    private string? _status;

    //   internal StreamingThreadRunOperation(
    //       ClientPipeline pipeline,
    //       Uri endpoint,
    //       RequestOptions? requestOptions,
    //       string threadId,
    //       string runId) : base(pipeline)
    //   {
    //       Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
    //       Argument.AssertNotNullOrEmpty(runId, nameof(runId));

    //       _threadId = threadId;
    //       _runId = runId;

    //       _endpoint = endpoint;
    //       _requestOptions = requestOptions;
    //}

    // Constructor used in protocol method - takes existing response
    internal StreamingThreadRunOperation(
        ClientPipeline pipeline,
        Uri endpoint,
        RequestOptions? requestOptions,
        string threadId,
        PipelineResponse response) : base(pipeline, response)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        //Argument.AssertNotNullOrEmpty(runId, nameof(runId));

        _threadId = threadId;

        // TODO: How to validate usage of this?
        _runId = default!;

        _endpoint = endpoint;
    }

    // Factory method - supports returning different OperationResult subtypes
    // from protocol method.
    public static StreamingThreadRunOperation FromResult(OperationResult result)
    {
        if (result is StreamingThreadRunOperation runOperation)
        {
            return runOperation;
        }

        throw new InvalidOperationException("Cannot create 'StreamingThreadRunOperation' from protocol 'OperationResult' when streaming response was not specified in request.");
    }

    public override async Task WaitForCompletionAsync()
    {
        PipelineResponse response = GetRawResponse();

        // TODO: switch based on whether response has been received yet or not
        Debug.Assert(response.ContentStream is not null);

        IAsyncEnumerable<SseItem<byte[]>> events = SseParser.Create(
                response.ContentStream!,
                (_, bytes) => bytes.ToArray()).EnumerateAsync();

        // TODO: plumb through cancellation token
        await foreach (SseItem<byte[]> item in events)
        {
            if (IsUpdateEvent(item))
            {
                ApplyUpdate(item);
            }
        }

        // Note: we should throw if user called this and we're in a state
        // where it can't be completed and can't be resumed.
        if (!HasCompleted)
        {
            throw new InvalidOperationException("Reached the end of the event stream without completing.  Consider calling WaitForStatusChange method instead.");
        }
    }

    public override void WaitForCompletion()
    {
        throw new NotImplementedException();
    }

    // Note: these have to work for protocol-only, so can't return the status.
    public async Task<string> WaitForStatusChangeAsync(RequestOptions? options)
    {
        //if (_eventStream is not null)
        //{
        //    throw new InvalidOperationException("Cannot stream events asynchronously after synchronous streaming has begun.");
        //}

        if (_asyncEventStream is null)
        {
            // TODO: request SSE stream if haven't yet

            _asyncEventStream = SseParser.Create(
                GetRawResponse().ContentStream!,
                (_, bytes) => bytes.ToArray()).EnumerateAsync();
        }

        CancellationToken cancellationToken = options?.CancellationToken ?? default;
        await foreach (SseItem<byte[]> item in _asyncEventStream.WithCancellation(cancellationToken))
        {
            if (IsUpdateEvent(item))
            {
                string? priorStatus = _status;
                ApplyUpdate(item);
                if (priorStatus != _status)
                {
                    return _status!;
                }
            }
        }

        return _status!;
    }

    // Returns state differently based on protocol vs convenience -- options vs.
    // cancellation token used to differentiate as in other clients.
    public string WaitForStatusChange(RequestOptions? options)
    {
        throw new NotImplementedException();
    }

    private bool IsUpdateEvent(SseItem<byte[]> item)
    {
        string name = item.EventType;

        bool isUpdateEvent =
            name == "thread.run.created" ||
            name == "thread.run.queued" ||
            name == "thread.run.in_progress" ||
            name == "thread.run.requires_action" ||
            name == "thread.run.cancelling" ||
            name == "thread.run.cancelled" ||
            name == "thread.run.failed" ||
            name == "thread.run.completed" ||
            name == "thread.run.incomplete" ||
            name == "thread.run.expired";

        return isUpdateEvent;
    }

    private void ApplyUpdate(SseItem<byte[]> update)
    {
        _status = GetStatus(update);
        HasCompleted = GetHasCompleted(_status);
    }

    private static string GetStatus(SseItem<byte[]> update)
    {
        // Take "thread.run." off the front of the name
        // TODO: perf
        return update.EventType.AsSpan().Slice("thread.run.".Length).ToString();
    }

    private static bool GetHasCompleted(string status)
    {
        bool hasCompleted =
            status == "expired" ||
            status == "completed" ||
            status == "failed" ||
            status == "incomplete" ||
            status == "cancelled";

        return hasCompleted;
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunAsync(RequestOptions? options)
    {
        using PipelineMessage message = CreateGetRunRequest(_threadId, _runId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRun(RequestOptions? options)
    {
        using PipelineMessage message = CreateGetRunRequest(_threadId, _runId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Modifies a run.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> ModifyRunAsync(BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyRunRequest(_threadId, _runId, content, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Modifies a run.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult ModifyRun(BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyRunRequest(_threadId, _runId, content, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Cancels a run that is `in_progress`.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CancelRunAsync(RequestOptions? options)
    {
        using PipelineMessage message = CreateCancelRunRequest(_threadId, _runId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Cancels a run that is `in_progress`.
    /// </summary>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CancelRun(RequestOptions? options)
    {
        using PipelineMessage message = CreateCancelRunRequest(_threadId, _runId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] When a run has the `status: "requires_action"` and `required_action.type` is
    /// `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once
    /// they're all completed. All outputs must be submitted in a single request.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> SubmitToolOutputsToRunAsync(BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage? message = null;
        try
        {
            message = CreateSubmitToolOutputsToRunRequest(_threadId, _runId, content, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message?.Dispose();
            }
        }
    }

    /// <summary>
    /// [Protocol Method] When a run has the `status: "requires_action"` and `required_action.type` is
    /// `submit_tool_outputs`, this endpoint can be used to submit the outputs from the tool calls once
    /// they're all completed. All outputs must be submitted in a single request.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult SubmitToolOutputsToRun(BinaryContent content, RequestOptions? options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        PipelineMessage? message = null;
        try
        {
            message = CreateSubmitToolOutputsToRunRequest(_threadId, _runId, content, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }
        finally
        {
            if (options?.BufferResponse != false)
            {
                message?.Dispose();
            }
        }
    }

    public virtual IAsyncEnumerable<ClientResult> GetRunStepsAsync(int? limit, string order, string after, string before, RequestOptions options)
    {
        PageResultEnumerator enumerator = new RunStepsPageEnumerator(Pipeline, _endpoint, _threadId, _runId, limit, order, after, before, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    public virtual IEnumerable<ClientResult> GetRunSteps(int? limit, string order, string after, string before, RequestOptions options)
    {
        PageResultEnumerator enumerator = new RunStepsPageEnumerator(Pipeline, _endpoint, _threadId, _runId, limit, order, after, before, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetRunStepAsync( string stepId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(stepId, nameof(stepId));

        using PipelineMessage message = CreateGetRunStepRequest(_threadId, _runId, stepId, options);
        return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves a run step.
    /// </summary>
    /// <param name="stepId"> The ID of the run step to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetRunStep(string stepId, RequestOptions? options)
    {
        Argument.AssertNotNullOrEmpty(stepId, nameof(stepId));

        using PipelineMessage message = CreateGetRunStepRequest(_threadId, _runId, stepId, options);
        return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
    }

    internal PipelineMessage CreateGetRunRequest(string threadId, string runId, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateModifyRunRequest(string threadId, string runId, BinaryContent content, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        request.Headers.Set("Content-Type", "application/json");
        request.Content = content;
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateCancelRunRequest(string threadId, string runId, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        uri.AppendPath("/cancel", false);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateSubmitToolOutputsToRunRequest(string threadId, string runId, BinaryContent content, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "POST";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        uri.AppendPath("/submit_tool_outputs", false);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        request.Headers.Set("Content-Type", "application/json");
        request.Content = content;
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateGetRunStepsRequest(string threadId, string runId, int? limit, string order, string after, string before, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        uri.AppendPath("/steps", false);
        if (limit != null)
        {
            uri.AppendQuery("limit", limit.Value, true);
        }
        if (order != null)
        {
            uri.AppendQuery("order", order, true);
        }
        if (after != null)
        {
            uri.AppendQuery("after", after, true);
        }
        if (before != null)
        {
            uri.AppendQuery("before", before, true);
        }
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    internal PipelineMessage CreateGetRunStepRequest(string threadId, string runId, string stepId, RequestOptions? options)
    {
        var message = Pipeline.CreateMessage();
        message.ResponseClassifier = PipelineMessageClassifier200;
        var request = message.Request;
        request.Method = "GET";
        var uri = new ClientUriBuilder();
        uri.Reset(_endpoint);
        uri.AppendPath("/threads/", false);
        uri.AppendPath(threadId, true);
        uri.AppendPath("/runs/", false);
        uri.AppendPath(runId, true);
        uri.AppendPath("/steps/", false);
        uri.AppendPath(stepId, true);
        request.Uri = uri.ToUri();
        request.Headers.Set("Accept", "application/json");
        message.Apply(options);
        return message;
    }

    private static PipelineMessageClassifier? _pipelineMessageClassifier200;
    private static PipelineMessageClassifier PipelineMessageClassifier200 => _pipelineMessageClassifier200 ??= PipelineMessageClassifier.Create(stackalloc ushort[] { 200 });
}