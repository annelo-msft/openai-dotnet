using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

public partial class AssistantClient
{
    /// <summary>
    /// [Protocol Method] Create an assistant with a model and instructions.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> CreateAssistantAsync(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateAssistantRequest(content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Create an assistant with a model and instructions.
    /// </summary>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult CreateAssistant(BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateCreateAssistantRequest(content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Returns a list of assistants.
    /// </summary>
    /// <param name="limit">
    /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    /// default is 20.
    /// </param>
    /// <param name="order">
    /// Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    /// for descending order. Allowed values: "asc" | "desc"
    /// </param>
    /// <param name="after">
    /// A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include after=obj_foo in order to fetch the next page of the list.
    /// </param>
    /// <param name="before">
    /// A cursor for use in pagination. `before` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual IAsyncEnumerable<ClientResult> GetAssistantsAsync(int? limit, string order, string after, string before, RequestOptions options)
    {
        AssistantsPageEnumerator enumerator = new AssistantsPageEnumerator(_pipeline, _endpoint, limit, order, after, before, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// [Protocol Method] Returns a list of assistants.
    /// </summary>
    /// <param name="limit">
    /// A limit on the number of objects to be returned. Limit can range between 1 and 100, and the
    /// default is 20.
    /// </param>
    /// <param name="order">
    /// Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and`desc`
    /// for descending order. Allowed values: "asc" | "desc"
    /// </param>
    /// <param name="after">
    /// A cursor for use in pagination. `after` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include after=obj_foo in order to fetch the next page of the list.
    /// </param>
    /// <param name="before">
    /// A cursor for use in pagination. `before` is an object ID that defines your place in the list.
    /// For instance, if you make a list request and receive 100 objects, ending with obj_foo, your
    /// subsequent call can include before=obj_foo in order to fetch the previous page of the list.
    /// </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual IEnumerable<ClientResult> GetAssistants(int? limit, string order, string after, string before, RequestOptions options)
    {
        AssistantsPageEnumerator enumerator = new AssistantsPageEnumerator(_pipeline, _endpoint, limit, order, after, before, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// [Protocol Method] Retrieves an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> GetAssistantAsync(string assistantId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        using PipelineMessage message = CreateGetAssistantRequest(assistantId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Retrieves an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to retrieve. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult GetAssistant(string assistantId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        using PipelineMessage message = CreateGetAssistantRequest(assistantId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Modifies an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> ModifyAssistantAsync(string assistantId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyAssistantRequest(assistantId, content, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Modifies an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to modify. </param>
    /// <param name="content"> The content to send as the body of the request. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> or <paramref name="content"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult ModifyAssistant(string assistantId, BinaryContent content, RequestOptions options = null)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        Argument.AssertNotNull(content, nameof(content));

        using PipelineMessage message = CreateModifyAssistantRequest(assistantId, content, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <summary>
    /// [Protocol Method] Delete an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual async Task<ClientResult> DeleteAssistantAsync(string assistantId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        using PipelineMessage message = CreateDeleteAssistantRequest(assistantId, options);
        return ClientResult.FromResponse(await _pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
    }

    /// <summary>
    /// [Protocol Method] Delete an assistant.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to delete. </param>
    /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="assistantId"/> is null. </exception>
    /// <exception cref="ArgumentException"> <paramref name="assistantId"/> is an empty string, and was expected to be non-empty. </exception>
    /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
    /// <returns> The response returned from the service. </returns>
    public virtual ClientResult DeleteAssistant(string assistantId, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        using PipelineMessage message = CreateDeleteAssistantRequest(assistantId, options);
        return ClientResult.FromResponse(_pipeline.ProcessMessage(message, options));
    }

    /// <inheritdoc cref="InternalAssistantMessageClient.CreateMessageAsync"/>
    public virtual Task<ClientResult> CreateMessageAsync(string threadId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.CreateMessageAsync(threadId, content, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.CreateMessage"/>
    public virtual ClientResult CreateMessage(string threadId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.CreateMessage(threadId, content, options);

    public virtual IAsyncEnumerable<ClientResult> GetMessagesAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        PageResultEnumerator enumerator = new MessagesPageEnumerator(_pipeline, _endpoint, threadId, limit, order, after, before, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    public virtual IEnumerable<ClientResult> GetMessages(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        PageResultEnumerator enumerator = new MessagesPageEnumerator(_pipeline, _endpoint, threadId, limit, order, after, before, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    /// <inheritdoc cref="InternalAssistantMessageClient.GetMessageAsync"/>
    public virtual Task<ClientResult> GetMessageAsync(string threadId, string messageId, RequestOptions options)
        => _messageSubClient.GetMessageAsync(threadId, messageId, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.GetMessage"/>
    public virtual ClientResult GetMessage(string threadId, string messageId, RequestOptions options)
        => _messageSubClient.GetMessage(threadId, messageId, options);
    /// <inheritdoc cref="InternalAssistantMessageClient.ModifyMessageAsync"/>
    public virtual Task<ClientResult> ModifyMessageAsync(string threadId, string messageId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.ModifyMessageAsync(threadId, messageId, content, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.ModifyMessage"/>
    public virtual ClientResult ModifyMessage(string threadId, string messageId, BinaryContent content, RequestOptions options = null)
        => _messageSubClient.ModifyMessage(threadId, messageId, content, options);
    /// <inheritdoc cref="InternalAssistantMessageClient.DeleteMessageAsync"/>
    public virtual Task<ClientResult> DeleteMessageAsync(string threadId, string messageId, RequestOptions options)
        => _messageSubClient.DeleteMessageAsync(threadId, messageId, options);

    /// <inheritdoc cref="InternalAssistantMessageClient.DeleteMessage"/>
    public virtual ClientResult DeleteMessage(string threadId, string messageId, RequestOptions options)
        => _messageSubClient.DeleteMessage(threadId, messageId, options);

    public virtual /*async*/ Task<ThreadRunOperation> CreateThreadAndRunAsync(
        ReturnWhen returnWhen,
        BinaryContent content,
        RequestOptions options = null)
    {
        throw new NotImplementedException();
        //ClientResult result = await _runSubClient.CreateThreadAndRunAsync(content, options).ConfigureAwait(false);

        //// Protocol level: get values needed to create subclient from response
        //PipelineResponse response = result.GetRawResponse();
        //using JsonDocument doc = JsonDocument.Parse(response.Content);
        //string threadId = doc.RootElement.GetProperty("thread_id"u8).GetString()!;
        //string runId = doc.RootElement.GetProperty("id"u8).GetString()!;

        //// Create the poller
        //ThreadRunPoller poller = new ThreadRunPoller(_pipeline, _endpoint, result, threadId, runId, options);

        //// Create the operation subclient
        //ThreadRunOperation operation = new ThreadRunOperation(
        //    _pipeline, _endpoint,
        //    threadId, runId, result.GetRawResponse(),
        //    poller);

        //if (returnWhen == ReturnWhen.Started)
        //{
        //    return operation;
        //}

        //operation.WaitForCompletionResult();
        //return operation;
    }

    public virtual ThreadRunOperation CreateThreadAndRun(
        ReturnWhen returnWhen,
        BinaryContent content,
        RequestOptions options = null)
    {
        throw new NotImplementedException();
        //ClientResult result = _runSubClient.CreateThreadAndRun(content, options);

        //// Protocol level: get values needed to create subclient from response
        //PipelineResponse response = result.GetRawResponse();
        //using JsonDocument doc = JsonDocument.Parse(response.Content);
        //string threadId = doc.RootElement.GetProperty("thread_id"u8).GetString()!;
        //string runId = doc.RootElement.GetProperty("id"u8).GetString()!;

        //// Create the poller
        //ThreadRunPoller poller = new ThreadRunPoller(_pipeline, _endpoint, result, threadId, runId, options);

        //// Create the operation subclient
        //ThreadRunOperation operation = new ThreadRunOperation(
        //    _pipeline, _endpoint,
        //    threadId, runId, result.GetRawResponse(),
        //    poller);

        //if (returnWhen == ReturnWhen.Started)
        //{
        //    return operation;
        //}

        //operation.WaitForCompletionResult();
        //return operation;
    }

    public virtual IAsyncEnumerable<ClientResult> GetRunsAsync(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        PageResultEnumerator enumerator = new RunsPageEnumerator(_pipeline, _endpoint, threadId, limit, order, after, before, options);
        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    public virtual IEnumerable<ClientResult> GetRuns(string threadId, int? limit, string order, string after, string before, RequestOptions options)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        PageResultEnumerator enumerator = new RunsPageEnumerator(_pipeline, _endpoint, threadId, limit, order, after, before, options);
        return PageCollectionHelpers.Create(enumerator);
    }

    public virtual async Task<ThreadRunOperation> CreateRunAsync(
        string threadId,
        BinaryContent content,
        RequestOptions options = null)
    {
        ClientResult result = await _runSubClient.CreateRunAsync(threadId, content, options).ConfigureAwait(false);
        PipelineResponse response = result.GetRawResponse();
        return new ThreadRunOperation(_pipeline, _endpoint, response);

        //// Is it polling or streaming?
        //if (!response.Headers.TryGetValue("Content-Type", out string contentType))
        //{
        //    throw new ClientResultException("Response did not contain 'Content-Type' header.");
        //}

        //return contentType switch
        //{
        //    "application/json" => new ThreadRunOperation(_pipeline, _endpoint, options, threadId, response),
        //    "text/event-stream; charset=utf-8" => new StreamingThreadRunOperation(_pipeline, _endpoint, options, threadId, response),
        //    _ => throw new ClientResultException($"Unexpected 'Content-Type' header value: '{contentType}'.")
        //};
    }

    public virtual ThreadRunOperation CreateRun(
        string threadId,
        BinaryContent content,
        RequestOptions options = null)
    {
        ClientResult result = _runSubClient.CreateRun(threadId, content, options);
        PipelineResponse response = result.GetRawResponse();
        return new ThreadRunOperation(_pipeline, _endpoint, response);

        //ClientResult result = _runSubClient.CreateRun(threadId, content, options);

        //// Protocol level: get values needed to create subclient from response
        //PipelineResponse response = result.GetRawResponse();
        //using JsonDocument doc = JsonDocument.Parse(response.Content);
        //string runId = doc.RootElement.GetProperty("id"u8).GetString()!;

        //// Is it polling or streaming?
        //if (!response.Headers.TryGetValue("Content-Type", out string contentType))
        //{
        //    throw new ClientResultException("Response did not contain 'Content-Type' header.");
        //}

        //return contentType switch
        //{
        //    "application/json" => new ThreadRunOperation(_pipeline, _endpoint, options, threadId, runId, response),
        //    "text/event-stream; charset=utf-8" => new StreamingThreadRunOperation(),
        //    _ => throw new ClientResultException($"Unexpected 'Content-Type' header value: '{contentType}'.")
        //};


        //// TODO: clean up poller and operation subclients per redundancy

        //// Create the poller
        //ThreadRunPoller poller = new ThreadRunPoller(
        //    _pipeline, _endpoint, result, threadId, runId, options);

        //// Create the operation subclient
        //ThreadRunOperation operation = new ThreadRunOperation(
        //    _pipeline, _endpoint,
        //    threadId, runId, result.GetRawResponse(),
        //    poller);

        //if (returnWhen == ReturnWhen.Started)
        //{
        //    return operation;
        //}

        //operation.WaitForCompletionResult();
        //return operation;
    }

    /// <inheritdoc cref="InternalAssistantThreadClient.CreateThreadAsync"/>
    public virtual Task<ClientResult> CreateThreadAsync(BinaryContent content, RequestOptions options = null)
        => _threadSubClient.CreateThreadAsync(content, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.CreateThread"/>
    public virtual ClientResult CreateThread(BinaryContent content, RequestOptions options = null)
        => _threadSubClient.CreateThread(content, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.GetThreadAsync"/>
    public virtual Task<ClientResult> GetThreadAsync(string threadId, RequestOptions options)
        => _threadSubClient.GetThreadAsync(threadId, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.GetThread"/>
    public virtual ClientResult GetThread(string threadId, RequestOptions options)
        => _threadSubClient.GetThread(threadId, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.ModifyThreadAsync"/>
    public virtual Task<ClientResult> ModifyThreadAsync(string threadId, BinaryContent content, RequestOptions options = null)
        => _threadSubClient.ModifyThreadAsync(threadId, content, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.ModifyThread"/>
    public virtual ClientResult ModifyThread(string threadId, BinaryContent content, RequestOptions options = null)
        => _threadSubClient.ModifyThread(threadId, content, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.DeleteThreadAsync"/>
    public virtual Task<ClientResult> DeleteThreadAsync(string threadId, RequestOptions options)
        => _threadSubClient.DeleteThreadAsync(threadId, options);

    /// <inheritdoc cref="InternalAssistantThreadClient.DeleteThread"/>
    public virtual ClientResult DeleteThread(string threadId, RequestOptions options)
        => _threadSubClient.DeleteThread(threadId, options);
}
