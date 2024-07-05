using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

/// <summary>
/// The service client for OpenAI assistants.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenClient("Assistants")]
[CodeGenSuppress("AssistantClient", typeof(ClientPipeline), typeof(ApiKeyCredential), typeof(Uri))]
[CodeGenSuppress("CreateAssistantAsync", typeof(AssistantCreationOptions))]
[CodeGenSuppress("CreateAssistant", typeof(AssistantCreationOptions))]
[CodeGenSuppress("DeleteAssistantAsync", typeof(string))]
[CodeGenSuppress("DeleteAssistant", typeof(string))]
[CodeGenSuppress("GetAssistantsAsync", typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
[CodeGenSuppress("GetAssistants", typeof(int?), typeof(ListOrder?), typeof(string), typeof(string))]
public partial class AssistantClient
{
    private readonly InternalAssistantMessageClient _messageSubClient;
    private readonly InternalAssistantRunClient _runSubClient;
    private readonly InternalAssistantThreadClient _threadSubClient;

    /// <summary>
    /// Initializes a new instance of <see cref="AssistantClient"/> that will use an API key when authenticating.
    /// </summary>
    /// <param name="credential"> The API key used to authenticate with the service endpoint. </param>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="ArgumentNullException"> The provided <paramref name="credential"/> was null. </exception>
    public AssistantClient(ApiKeyCredential credential, OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(credential, requireExplicitCredential: true), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="AssistantClient"/> that will use an API key from the OPENAI_API_KEY
    /// environment variable when authenticating.
    /// </summary>
    /// <remarks>
    /// To provide an explicit credential instead of using the environment variable, use an alternate constructor like
    /// <see cref="AssistantClient(ApiKeyCredential,OpenAIClientOptions)"/>.
    /// </remarks>
    /// <param name="options"> Additional options to customize the client. </param>
    /// <exception cref="InvalidOperationException"> The OPENAI_API_KEY environment variable was not found. </exception>
    public AssistantClient(OpenAIClientOptions options = default)
        : this(
              OpenAIClient.CreatePipeline(OpenAIClient.GetApiKey(), options),
              OpenAIClient.GetEndpoint(options),
              options)
    { }

    /// <summary> Initializes a new instance of <see cref="AssistantClient"/>. </summary>
    /// <param name="pipeline"> The HTTP pipeline for sending and receiving REST requests and responses. </param>
    /// <param name="endpoint"> OpenAI Endpoint. </param>
    /// <param name="options"> Client-wide options to propagate settings from. </param>
    protected internal AssistantClient(ClientPipeline pipeline, Uri endpoint, OpenAIClientOptions options)
    {
        _pipeline = pipeline;
        _endpoint = endpoint;
        _messageSubClient = new(_pipeline, _endpoint, options);
        _runSubClient = new(_pipeline, _endpoint, options);
        _threadSubClient = new(_pipeline, _endpoint, options);
    }

    /// <summary> Creates a new assistant. </summary>
    /// <param name="model"> The default model that the assistant should use. </param>
    /// <param name="options"> The additional <see cref="AssistantCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is null or empty. </exception>
    public virtual async Task<ClientResult<Assistant>> CreateAssistantAsync(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new();
        options.Model = model;

        ClientResult protocolResult = await CreateAssistantAsync(options?.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, Assistant.FromResponse);
    }

    /// <summary> Creates a new assistant. </summary>
    /// <param name="model"> The default model that the assistant should use. </param>
    /// <param name="options"> The additional <see cref="AssistantCreationOptions"/> to use. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <exception cref="ArgumentException"> <paramref name="model"/> is null or empty. </exception>
    public virtual ClientResult<Assistant> CreateAssistant(string model, AssistantCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(model, nameof(model));
        options ??= new();
        options.Model = model;

        ClientResult protocolResult = CreateAssistant(options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, Assistant.FromResponse);
    }

    /// <summary>
    /// Returns a collection of <see cref="Assistant"/> instances.
    /// </summary>
    /// <param name="options">Options describing the collection to return.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual AsyncPageCollection<Assistant> GetAssistantsAsync(
        AssistantCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        AssistantsPageEnumerator enumerator = new(_pipeline, _endpoint,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// Rehydrates a collection of <see cref="Assistant"/> instances a page token's serialized bytes.
    /// </summary>
    /// <param name="firstPageToken">Serialized page token indicating the first page of the collection to rehydrate.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual AsyncPageCollection<Assistant> GetAssistantsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        AssistantsPageToken pageToken = AssistantsPageToken.FromToken(firstPageToken);
        AssistantsPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// Returns a collection of <see cref="Assistant"/> instances.
    /// </summary>
    /// <param name="options">Options describing the collection to return.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual PageCollection<Assistant> GetAssistants(
        AssistantCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        AssistantsPageEnumerator enumerator = new(_pipeline, _endpoint,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// Rehydrates a collection of <see cref="Assistant"/> instances a page token's serialized bytes.
    /// </summary>
    /// <param name="firstPageToken">Serialized page token indicating the first page of the collection to rehydrate.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual PageCollection<Assistant> GetAssistants(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        AssistantsPageToken pageToken = AssistantsPageToken.FromToken(firstPageToken);
        AssistantsPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// Deletes an existing <see cref="Assistant"/>. 
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual async Task<ClientResult<bool>> DeleteAssistantAsync(string assistantId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        ClientResult protocolResult = await DeleteAssistantAsync(assistantId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, response
            => InternalDeleteAssistantResponse.FromResponse(response).Deleted);
    }

    /// <summary>
    /// Deletes an existing <see cref="Assistant"/>. 
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteAssistant(string assistantId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

        ClientResult protocolResult = DeleteAssistant(assistantId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, response
            => InternalDeleteAssistantResponse.FromResponse(response).Deleted);
    }

    /// <summary>
    /// Creates a new <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="options"> Additional options to use when creating the thread. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new thread. </returns>
    public virtual async Task<ClientResult<AssistantThread>> CreateThreadAsync(ThreadCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = await CreateThreadAsync(options?.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, AssistantThread.FromResponse);
    }

    /// <summary>
    /// Creates a new <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="options"> Additional options to use when creating the thread. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new thread. </returns>
    public virtual ClientResult<AssistantThread> CreateThread(ThreadCreationOptions options = null, CancellationToken cancellationToken = default)
    {
        ClientResult protocolResult = CreateThread(options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, AssistantThread.FromResponse);
    }

    /// <summary>
    /// Gets an existing <see cref="AssistantThread"/>, retrieved via a known ID.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing thread instance. </returns>
    public virtual async Task<ClientResult<AssistantThread>> GetThreadAsync(string threadId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        ClientResult protocolResult = await GetThreadAsync(threadId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, AssistantThread.FromResponse);
    }

    /// <summary>
    /// Gets an existing <see cref="AssistantThread"/>, retrieved via a known ID.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing thread instance. </returns>
    public virtual ClientResult<AssistantThread> GetThread(string threadId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        ClientResult protocolResult = GetThread(threadId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, AssistantThread.FromResponse);
    }

    /// <summary>
    /// Modifies an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to modify. </param>
    /// <param name="options"> The modifications to apply to the thread. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The updated <see cref="AssistantThread"/> instance. </returns>
    public virtual async Task<ClientResult<AssistantThread>> ModifyThreadAsync(string threadId, ThreadModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(options, nameof(options));

        ClientResult protocolResult = await ModifyThreadAsync(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, AssistantThread.FromResponse);
    }

    /// <summary>
    /// Modifies an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to modify. </param>
    /// <param name="options"> The modifications to apply to the thread. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The updated <see cref="AssistantThread"/> instance. </returns>
    public virtual ClientResult<AssistantThread> ModifyThread(string threadId, ThreadModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNull(options, nameof(options));

        ClientResult protocolResult = ModifyThread(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, AssistantThread.FromResponse);
    }

    /// <summary>
    /// Deletes an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual async Task<ClientResult<bool>> DeleteThreadAsync(string threadId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        ClientResult protocolResult = await DeleteThreadAsync(threadId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, response
            => InternalDeleteThreadResponse.FromResponse(response).Deleted);
    }

    /// <summary>
    /// Deletes an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to delete. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteThread(string threadId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        ClientResult protocolResult = DeleteThread(threadId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, response
            => InternalDeleteThreadResponse.FromResponse(response).Deleted);
    }

    /// <summary>
    /// Creates a new <see cref="ThreadMessage"/> on an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to associate the new message with. </param>
    /// <param name="role"> The role to associate with the new message. </param>
    /// <param name="content"> The collection of <see cref="MessageContent"/> items for the message. </param>
    /// <param name="options"> Additional options to apply to the new message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadMessage"/>. </returns>
    public virtual async Task<ClientResult<ThreadMessage>> CreateMessageAsync(
        string threadId,
        MessageRole role,
        IEnumerable<MessageContent> content,
        MessageCreationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        options ??= new();
        options.Role = role;
        options.Content.Clear();
        foreach (MessageContent contentItem in content)
        {
            options.Content.Add(contentItem);
        }

        ClientResult protocolResult = await CreateMessageAsync(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadMessage.FromResponse);
    }

    /// <summary>
    /// Creates a new <see cref="ThreadMessage"/> on an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to associate the new message with. </param>
    /// <param name="role"> The role to associate with the new message. </param>
    /// <param name="content"> The collection of <see cref="MessageContent"/> items for the message. </param>
    /// <param name="options"> Additional options to apply to the new message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> CreateMessage(
        string threadId,
        MessageRole role,
        IEnumerable<MessageContent> content,
        MessageCreationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        options ??= new();
        options.Role = role;
        options.Content.Clear();
        foreach (MessageContent contentItem in content)
        {
            options.Content.Add(contentItem);
        }

        ClientResult protocolResult = CreateMessage(threadId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadMessage.FromResponse);
    }

    /// <summary>
    /// Returns a collection of <see cref="ThreadMessage"/> instances from an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to list messages from. </param>
    /// <param name="options">Options describing the collection to return.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual AsyncPageCollection<ThreadMessage> GetMessagesAsync(
        string threadId,
        MessageCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        MessagesPageEnumerator enumerator = new(_pipeline, _endpoint,
            threadId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    public virtual AsyncPageCollection<ThreadMessage> GetMessagesAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        MessagesPageToken pageToken = MessagesPageToken.FromToken(firstPageToken);
        MessagesPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.ThreadId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// Returns a collection of <see cref="ThreadMessage"/> instances from an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to list messages from. </param>
    /// <param name="options">Options describing the collection to return.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual PageCollection<ThreadMessage> GetMessages(
        string threadId,
        MessageCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        MessagesPageEnumerator enumerator = new(_pipeline, _endpoint,
            threadId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    public virtual PageCollection<ThreadMessage> GetMessages(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        MessagesPageToken pageToken = MessagesPageToken.FromToken(firstPageToken);
        MessagesPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.ThreadId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadMessage"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve the message from. </param>
    /// <param name="messageId"> The ID of the message to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadMessage"/> instance. </returns>
    public virtual async Task<ClientResult<ThreadMessage>> GetMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        ClientResult protocolResult = await GetMessageAsync(threadId, messageId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadMessage.FromResponse);
    }

    /// <summary>
    /// Gets an existing <see cref="ThreadMessage"/> from a known <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread to retrieve the message from. </param>
    /// <param name="messageId"> The ID of the message to retrieve. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The existing <see cref="ThreadMessage"/> instance. </returns>
    public virtual ClientResult<ThreadMessage> GetMessage(string threadId, string messageId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        ClientResult protocolResult = GetMessage(threadId, messageId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadMessage.FromResponse);
    }

    /// <summary>
    /// Modifies an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the message to modify. </param>
    /// <param name="messageId"> The ID of the message to modify. </param>
    /// <param name="options"> The changes to apply to the message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The updated <see cref="ThreadMessage"/>. </returns>
    public virtual async Task<ClientResult<ThreadMessage>> ModifyMessageAsync(string threadId, string messageId, MessageModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));
        Argument.AssertNotNull(options, nameof(options));

        ClientResult protocolResult = await ModifyMessageAsync(threadId, messageId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions())
            .ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, ThreadMessage.FromResponse);
    }

    /// <summary>
    /// Modifies an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the message to modify. </param>
    /// <param name="messageId"> The ID of the message to modify. </param>
    /// <param name="options"> The changes to apply to the message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> The updated <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> ModifyMessage(string threadId, string messageId, MessageModificationOptions options, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));
        Argument.AssertNotNull(options, nameof(options));

        ClientResult protocolResult = ModifyMessage(threadId, messageId, options?.ToBinaryContent(), cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, ThreadMessage.FromResponse);
    }

    /// <summary>
    /// Deletes an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the message. </param>
    /// <param name="messageId"> The ID of the message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual async Task<ClientResult<bool>> DeleteMessageAsync(string threadId, string messageId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        ClientResult protocolResult = await DeleteMessageAsync(threadId, messageId, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
        return CreateResultFromProtocol(protocolResult, response =>
            InternalDeleteMessageResponse.FromResponse(response).Deleted);
    }

    /// <summary>
    /// Deletes an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread associated with the message. </param>
    /// <param name="messageId"> The ID of the message. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteMessage(string threadId, string messageId, CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(messageId, nameof(messageId));

        ClientResult protocolResult = DeleteMessage(threadId, messageId, cancellationToken.ToRequestOptions());
        return CreateResultFromProtocol(protocolResult, response =>
            InternalDeleteMessageResponse.FromResponse(response).Deleted);
    }

    /// <summary>
    /// Begins a new <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that the run should evaluate. </param>
    /// <param name="assistantId"> The ID of the assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadRun"/> instance. </returns>
    public virtual async Task<ThreadRunOperation> CreateRunAsync(
        ReturnWhen returnWhen,
        string threadId,
        string assistantId,
        RunCreationOptions options = null,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        options ??= new();
        options.AssistantId = assistantId;
        options.Stream = null;

        return await CreateRunAsync(returnWhen, threadId, options.ToBinaryContent(), cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }

    /// <summary>
    /// Begins a new <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that the run should evaluate. </param>
    /// <param name="assistantId"> The ID of the assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadRun"/> instance. </returns>
    public virtual ThreadRunOperation CreateRun(
        ReturnWhen returnWhen,
        string threadId, 
        string assistantId, 
        RunCreationOptions options = null, 
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        options ??= new();
        options.AssistantId = assistantId;
        options.Stream = null;

        return CreateRun(returnWhen, threadId, options.ToBinaryContent(), cancellationToken.ToRequestOptions());
    }

    ///// <summary>
    ///// Begins a new streaming <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    ///// <see cref="Assistant"/>.
    ///// </summary>
    ///// <param name="threadId"> The ID of the thread that the run should evaluate. </param>
    ///// <param name="assistantId"> The ID of the assistant that should be used when evaluating the thread. </param>
    ///// <param name="options"> Additional options for the run. </param>
    ///// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    //public virtual AsyncCollectionResult<StreamingUpdate> CreateRunStreamingAsync(
    //    string threadId,
    //    string assistantId,
    //    RunCreationOptions options = null,
    //    CancellationToken cancellationToken = default)
    //{
    //    Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
    //    Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

    //    options ??= new();
    //    options.AssistantId = assistantId;
    //    options.Stream = true;

    //    async Task<ClientResult> getResultAsync() =>
    //        await CreateRunAsync(threadId, options.ToBinaryContent(), cancellationToken.ToRequestOptions(streaming: true))
    //        .ConfigureAwait(false);

    //    return new AsyncStreamingUpdateCollection(getResultAsync);
    //}

    ///// <summary>
    ///// Begins a new streaming <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    ///// <see cref="Assistant"/>.
    ///// </summary>
    ///// <param name="threadId"> The ID of the thread that the run should evaluate. </param>
    ///// <param name="assistantId"> The ID of the assistant that should be used when evaluating the thread. </param>
    ///// <param name="options"> Additional options for the run. </param>
    ///// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    //public virtual CollectionResult<StreamingUpdate> CreateRunStreaming(
    //    string threadId,
    //    string assistantId,
    //    RunCreationOptions options = null,
    //    CancellationToken cancellationToken = default)
    //{
    //    Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));
    //    Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

    //    options ??= new();
    //    options.AssistantId = assistantId;
    //    options.Stream = true;

    //    ClientResult getResult() => CreateRun(threadId, options.ToBinaryContent(), cancellationToken.ToRequestOptions(streaming: true));

    //    return new StreamingUpdateCollection(getResult);
    //}

    /// <summary>
    /// Creates a new thread and immediately begins a run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadRun"/>. </returns>
    public virtual async Task<ThreadRunOperation> CreateThreadAndRunAsync(
        ReturnWhen returnWhen,
        string assistantId,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null,
        CancellationToken cancellationToken = default)
    {
        runOptions ??= new();
        runOptions.Stream = null;
        BinaryContent protocolContent = CreateThreadAndRunProtocolContent(assistantId, threadOptions, runOptions);
        return await CreateThreadAndRunAsync(returnWhen, protocolContent, cancellationToken.ToRequestOptions()).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new thread and immediately begins a run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistantId"> The ID of the assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns> A new <see cref="ThreadRun"/>. </returns>
    public virtual ThreadRunOperation CreateThreadAndRun(
        ReturnWhen returnWhen,
        string assistantId,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null,
        CancellationToken cancellationToken = default)
    {
        runOptions ??= new();
        runOptions.Stream = null;
        BinaryContent protocolContent = CreateThreadAndRunProtocolContent(assistantId, threadOptions, runOptions);
        return CreateThreadAndRun(returnWhen, protocolContent, cancellationToken.ToRequestOptions());
    }

    ///// <summary>
    ///// Creates a new thread and immediately begins a streaming run against it using the specified <see cref="Assistant"/>.
    ///// </summary>
    ///// <param name="assistantId"> The ID of the assistant that the new run should use. </param>
    ///// <param name="threadOptions"> Options for the new thread that will be created. </param>
    ///// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    ///// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    //public virtual AsyncCollectionResult<StreamingUpdate> CreateThreadAndRunStreamingAsync(
    //    string assistantId,
    //    ThreadCreationOptions threadOptions = null,
    //    RunCreationOptions runOptions = null,
    //    CancellationToken cancellationToken = default)
    //{
    //    Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

    //    runOptions ??= new();
    //    runOptions.Stream = true;
    //    BinaryContent protocolContent = CreateThreadAndRunProtocolContent(assistantId, threadOptions, runOptions);

    //    async Task<ClientResult> getResultAsync() =>
    //        await CreateThreadAndRunAsync(protocolContent, cancellationToken.ToRequestOptions(streaming: true))
    //        .ConfigureAwait(false);

    //    return new AsyncStreamingUpdateCollection(getResultAsync);
    //}

    ///// <summary>
    ///// Creates a new thread and immediately begins a streaming run against it using the specified <see cref="Assistant"/>.
    ///// </summary>
    ///// <param name="assistantId"> The ID of the assistant that the new run should use. </param>
    ///// <param name="threadOptions"> Options for the new thread that will be created. </param>
    ///// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    ///// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    //public virtual CollectionResult<StreamingUpdate> CreateThreadAndRunStreaming(
    //    string assistantId,
    //    ThreadCreationOptions threadOptions = null,
    //    RunCreationOptions runOptions = null,
    //    CancellationToken cancellationToken = default)
    //{
    //    Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));

    //    runOptions ??= new();
    //    runOptions.Stream = true;
    //    BinaryContent protocolContent = CreateThreadAndRunProtocolContent(assistantId, threadOptions, runOptions);

    //    ClientResult getResult() => CreateThreadAndRun(protocolContent, cancellationToken.ToRequestOptions(streaming: true));

    //    return new StreamingUpdateCollection(getResult);
    //}

    /// <summary>
    /// Returns a collection of <see cref="ThreadRun"/> instances associated with an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that runs in the list should be associated with. </param>
    /// <param name="options">Options describing the collection to return.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual AsyncPageCollection<ThreadRun> GetRunsAsync(
        string threadId,
        RunCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        RunsPageEnumerator enumerator = new(_pipeline, _endpoint,
            threadId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    public virtual AsyncPageCollection<ThreadRun> GetRunsAsync(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        RunsPageToken pageToken = RunsPageToken.FromToken(firstPageToken);
        RunsPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.ThreadId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.CreateAsync(enumerator);
    }

    /// <summary>
    /// Returns a collection of <see cref="ThreadRun"/> instances associated with an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="threadId"> The ID of the thread that runs in the list should be associated with. </param>
    /// <param name="options">Options describing the collection to return.</param>
    /// <param name="cancellationToken">A token that can be used to cancel this method call.</param>
    /// <returns></returns>
    public virtual PageCollection<ThreadRun> GetRuns(
        string threadId,
        RunCollectionOptions options = default,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNullOrEmpty(threadId, nameof(threadId));

        RunsPageEnumerator enumerator = new(_pipeline, _endpoint,
            threadId,
            options?.PageSize,
            options?.Order?.ToString(),
            options?.AfterId,
            options?.BeforeId,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    public virtual PageCollection<ThreadRun> GetRuns(
        ContinuationToken firstPageToken,
        CancellationToken cancellationToken = default)
    {
        Argument.AssertNotNull(firstPageToken, nameof(firstPageToken));

        RunsPageToken pageToken = RunsPageToken.FromToken(firstPageToken);
        RunsPageEnumerator enumerator = new(_pipeline, _endpoint,
            pageToken.ThreadId,
            pageToken.Limit,
            pageToken.Order,
            pageToken.After,
            pageToken.Before,
            cancellationToken.ToRequestOptions());

        return PageCollectionHelpers.Create(enumerator);
    }

    private static BinaryContent CreateThreadAndRunProtocolContent(
        string assistantId,
        ThreadCreationOptions threadOptions,
        RunCreationOptions runOptions)
    {
        Argument.AssertNotNullOrEmpty(assistantId, nameof(assistantId));
        InternalCreateThreadAndRunRequest internalRequest = new(
            assistantId,
            threadOptions,
            runOptions.ModelOverride,
            runOptions.InstructionsOverride,
            runOptions.ToolsOverride,
            // TODO: reconcile exposure of the the two different tool_resources, if needed
            threadOptions?.ToolResources,
            runOptions.Metadata,
            runOptions.Temperature,
            runOptions.NucleusSamplingFactor,
            runOptions.Stream,
            runOptions.MaxPromptTokens,
            runOptions.MaxCompletionTokens,
            runOptions.TruncationStrategy,
            runOptions.ToolConstraint,
            runOptions.ParallelToolCallsEnabled,
            runOptions.ResponseFormat,
            serializedAdditionalRawData: null);
        return internalRequest.ToBinaryContent();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ClientResult<T> CreateResultFromProtocol<T>(ClientResult protocolResult, Func<PipelineResponse, T> responseDeserializer)
    {
        PipelineResponse pipelineResponse = protocolResult?.GetRawResponse();
        T deserializedResultValue = responseDeserializer.Invoke(pipelineResponse);
        return ClientResult.FromValue(deserializedResultValue, pipelineResponse);
    }
}
