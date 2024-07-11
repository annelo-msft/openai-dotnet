using System.ClientModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenAI.Assistants;

public partial class AssistantClient
{
    /// <summary>
    /// Modifies an existing <see cref="Assistant"/>. 
    /// </summary>
    /// <param name="assistant"> The assistant to modify. </param>
    /// <param name="options"> The changes to apply to the assistant. </param>
    /// <returns>
    /// An updated <see cref="Assistant"/> instance that reflects the requested changes. 
    /// </returns>
    public virtual Task<ClientResult<Assistant>> ModifyAssistantAsync(Assistant assistant, AssistantModificationOptions options)
        => ModifyAssistantAsync(assistant?.Id, options);

    /// <summary>
    /// Modifies an existing <see cref="Assistant"/>. 
    /// </summary>
    /// <param name="assistant"> The assistant to modify. </param>
    /// <param name="options"> The changes to apply to the assistant. </param>
    /// <returns>
    /// An updated <see cref="Assistant"/> instance that reflects the requested changes. 
    /// </returns>
    public virtual ClientResult<Assistant> ModifyAssistant(Assistant assistant, AssistantModificationOptions options)
        => ModifyAssistant(assistant?.Id, options);

    /// <summary>
    /// Deletes an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant to delete. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual Task<ClientResult<bool>> DeleteAssistantAsync(Assistant assistant)
        => DeleteAssistantAsync(assistant?.Id);

    /// <summary>
    /// Deletes an existing <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant to delete. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteAssistant(Assistant assistant)
        => DeleteAssistant(assistant?.Id);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The existing thread to refresh the state of. </param>
    /// <returns> An updated instance of the provided <see cref="ThreadMessage"/>. </returns>
    public virtual Task<ClientResult<AssistantThread>> GetThreadAsync(AssistantThread thread)
        => GetThreadAsync(thread?.Id);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The existing thread to refresh the state of. </param>
    /// <returns> An updated instance of the provided <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<AssistantThread> GetThread(AssistantThread thread)
        => GetThread(thread?.Id);

    /// <summary>
    /// Modifies an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to modify. </param>
    /// <param name="options"> The modifications to apply to the thread. </param>
    /// <returns> The updated <see cref="AssistantThread"/> instance. </returns>
    public virtual Task<ClientResult<AssistantThread>> ModifyThreadAsync(AssistantThread thread, ThreadModificationOptions options)
        => ModifyThreadAsync(thread?.Id, options);

    /// <summary>
    /// Modifies an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to modify. </param>
    /// <param name="options"> The modifications to apply to the thread. </param>
    /// <returns> The updated <see cref="AssistantThread"/> instance. </returns>
    public virtual ClientResult<AssistantThread> ModifyThread(AssistantThread thread, ThreadModificationOptions options)
        => ModifyThread(thread?.Id, options);

    /// <summary>
    /// Deletes an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to delete. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual Task<ClientResult<bool>> DeleteThreadAsync(AssistantThread thread)
        => DeleteThreadAsync(thread?.Id);

    /// <summary>
    /// Deletes an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to delete. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteThread(AssistantThread thread)
        => DeleteThread(thread?.Id);

    /// <summary>
    /// Creates a new <see cref="ThreadMessage"/> on an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to associate the new message with. </param>
    /// <param name="role"> The role to associate with the new message. </param>
    /// <param name="content"> The collection of <see cref="MessageContent"/> items for the message. </param>
    /// <param name="options"> Additional options to apply to the new message. </param>
    /// <returns> A new <see cref="ThreadMessage"/>. </returns>
    public virtual Task<ClientResult<ThreadMessage>> CreateMessageAsync(
        AssistantThread thread,
        MessageRole role,
        IEnumerable<MessageContent> content,
        MessageCreationOptions options = null)
            => CreateMessageAsync(thread?.Id, role, content, options);

    /// <summary>
    /// Creates a new <see cref="ThreadMessage"/> on an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to associate the new message with. </param>
    /// <param name="role"> The role to associate with the new message. </param>
    /// <param name="content"> The collection of <see cref="MessageContent"/> items for the message. </param>
    /// <param name="options"> Additional options to apply to the new message. </param>
    /// <returns> A new <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> CreateMessage(
        AssistantThread thread,
        MessageRole role,
        IEnumerable<MessageContent> content,
        MessageCreationOptions options = null)
            => CreateMessage(thread?.Id, role, content, options);

    /// <summary>
    /// Returns a collection of <see cref="ThreadMessage"/> instances from an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to list messages from. </param>
    /// <param name="options">Options describing the collection to return.</param>
    /// <returns></returns>
    public virtual AsyncPageCollection<ThreadMessage> GetMessagesAsync(
        AssistantThread thread,
        MessageCollectionOptions options = default)
    {
        Argument.AssertNotNull(thread, nameof(thread));

        return GetMessagesAsync(thread.Id, options);
    }

    /// <summary>
    /// Returns a collection of <see cref="ThreadMessage"/> instances from an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread to list messages from. </param>
    /// <param name="options">Options describing the collection to return.</param>
    /// <returns></returns>
    public virtual PageCollection<ThreadMessage> GetMessages(
        AssistantThread thread, 
        MessageCollectionOptions options = default)
    {
        Argument.AssertNotNull(thread, nameof(thread));

        return GetMessages(thread.Id, options);
    }

    /// <summary>
    /// Gets an updated instance of an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The existing message to refresh the state of. </param>
    /// <returns> An updated instance of the provided <see cref="ThreadMessage"/>. </returns>
    public virtual Task<ClientResult<ThreadMessage>> GetMessageAsync(ThreadMessage message)
        => GetMessageAsync(message?.ThreadId, message?.Id);

    /// <summary>
    /// Gets an updated instance of an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The existing message to refresh the state of. </param>
    /// <returns> An updated instance of the provided <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> GetMessage(ThreadMessage message)
        => GetMessage(message?.ThreadId, message?.Id);

    /// <summary>
    /// Modifies an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The message to modify. </param>
    /// <param name="options"> The changes to apply to the message. </param>
    /// <returns> The updated <see cref="ThreadMessage"/>. </returns>
    public virtual Task<ClientResult<ThreadMessage>> ModifyMessageAsync(ThreadMessage message, MessageModificationOptions options)
        => ModifyMessageAsync(message?.ThreadId, message?.Id, options);

    /// <summary>
    /// Modifies an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The message to modify. </param>
    /// <param name="options"> The changes to apply to the message. </param>
    /// <returns> The updated <see cref="ThreadMessage"/>. </returns>
    public virtual ClientResult<ThreadMessage> ModifyMessage(ThreadMessage message, MessageModificationOptions options)
        => ModifyMessage(message?.ThreadId, message?.Id, options);

    /// <summary>
    /// Deletes an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The message to delete. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual Task<ClientResult<bool>> DeleteMessageAsync(ThreadMessage message)
        => DeleteMessageAsync(message?.ThreadId, message?.Id);

    /// <summary>
    /// Deletes an existing <see cref="ThreadMessage"/>.
    /// </summary>
    /// <param name="message"> The message to delete. </param>
    /// <returns> A value indicating whether the deletion was successful. </returns>
    public virtual ClientResult<bool> DeleteMessage(ThreadMessage message)
        => DeleteMessage(message?.ThreadId, message?.Id);

    /// <summary>
    /// Begins a new <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="thread"> The thread that the run should evaluate. </param>
    /// <param name="assistant"> The assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <returns> A new <see cref="ThreadRun"/> instance. </returns>
    public virtual async Task<ThreadRunOperation> CreateRunAsync(
        ReturnWhen returnWhen,
        AssistantThread thread,
        Assistant assistant,
        RunCreationOptions options = null)
        => await CreateRunAsync(returnWhen, thread?.Id, assistant?.Id, options).ConfigureAwait(false);

    /// <summary>
    /// Begins a new <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    /// <see cref="Assistant"/>.
    /// </summary>
    /// <param name="thread"> The thread that the run should evaluate. </param>
    /// <param name="assistant"> The assistant that should be used when evaluating the thread. </param>
    /// <param name="options"> Additional options for the run. </param>
    /// <returns> A new <see cref="ThreadRun"/> instance. </returns>
    public virtual ThreadRunOperation CreateRun(
        ReturnWhen returnWhen,
        AssistantThread thread, 
        Assistant assistant, 
        RunCreationOptions options = null)
        => CreateRun(returnWhen, thread?.Id, assistant?.Id, options);

    ///// <summary>
    ///// Begins a new streaming <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    ///// <see cref="Assistant"/>.
    ///// </summary>
    ///// <param name="thread"> The thread that the run should evaluate. </param>
    ///// <param name="assistant"> The assistant that should be used when evaluating the thread. </param>
    ///// <param name="options"> Additional options for the run. </param>
    //public virtual StreamingThreadRunOperation CreateRunStreamingAsync(
    //    AssistantThread thread,
    //    Assistant assistant,
    //    RunCreationOptions options = null)
    //        => CreateRunStreamingAsync(thread?.Id, assistant?.Id, options);

    ///// <summary>
    ///// Begins a new streaming <see cref="ThreadRun"/> that evaluates a <see cref="AssistantThread"/> using a specified
    ///// <see cref="Assistant"/>.
    ///// </summary>
    ///// <param name="thread"> The thread that the run should evaluate. </param>
    ///// <param name="assistant"> The assistant that should be used when evaluating the thread. </param>
    ///// <param name="options"> Additional options for the run. </param>
    //public virtual CollectionResult<StreamingUpdate> CreateRunStreaming(
    //    AssistantThread thread,
    //    Assistant assistant,
    //    RunCreationOptions options = null)
    //        => CreateRunStreaming(thread?.Id, assistant?.Id, options);

    /// <summary>
    /// Creates a new thread and immediately begins a run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <returns> A new <see cref="ThreadRun"/>. </returns>
    public virtual async Task<ThreadRunOperation> CreateThreadAndRunAsync(
        ReturnWhen returnWhen,
        Assistant assistant,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null)
            => await CreateThreadAndRunAsync(returnWhen, assistant?.Id, threadOptions, runOptions).ConfigureAwait(false);

    /// <summary>
    /// Creates a new thread and immediately begins a run against it using the specified <see cref="Assistant"/>.
    /// </summary>
    /// <param name="assistant"> The assistant that the new run should use. </param>
    /// <param name="threadOptions"> Options for the new thread that will be created. </param>
    /// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    /// <returns> A new <see cref="ThreadRun"/>. </returns>
    public virtual ThreadRunOperation CreateThreadAndRun(
        ReturnWhen returnWhen,
        Assistant assistant,
        ThreadCreationOptions threadOptions = null,
        RunCreationOptions runOptions = null)
            => CreateThreadAndRun(returnWhen, assistant?.Id, threadOptions, runOptions);

    ///// <summary>
    ///// Creates a new thread and immediately begins a streaming run against it using the specified <see cref="Assistant"/>.
    ///// </summary>
    ///// <param name="assistant"> The assistant that the new run should use. </param>
    ///// <param name="threadOptions"> Options for the new thread that will be created. </param>
    ///// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    //public virtual AsyncCollectionResult<StreamingUpdate> CreateThreadAndRunStreamingAsync(
    //    Assistant assistant,
    //    ThreadCreationOptions threadOptions = null,
    //    RunCreationOptions runOptions = null)
    //        => CreateThreadAndRunStreamingAsync(assistant?.Id, threadOptions, runOptions);

    ///// <summary>
    ///// Creates a new thread and immediately begins a streaming run against it using the specified <see cref="Assistant"/>.
    ///// </summary>
    ///// <param name="assistant"> The assistant that the new run should use. </param>
    ///// <param name="threadOptions"> Options for the new thread that will be created. </param>
    ///// <param name="runOptions"> Additional options to apply to the run that will begin. </param>
    //public virtual CollectionResult<StreamingUpdate> CreateThreadAndRunStreaming(
    //    Assistant assistant,
    //    ThreadCreationOptions threadOptions = null,
    //    RunCreationOptions runOptions = null)
    //        => CreateThreadAndRunStreaming(assistant?.Id, threadOptions, runOptions);

    /// <summary>
    /// Returns a collection of <see cref="ThreadRun"/> instances associated with an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread that runs in the list should be associated with. </param>
    /// <param name="options">Options describing the collection to return.</param>
    /// <returns></returns>
    public virtual AsyncPageCollection<ThreadRun> GetRunsAsync(
        AssistantThread thread,
        RunCollectionOptions options = default)
    {
        Argument.AssertNotNull(thread, nameof(thread));

        return GetRunsAsync(thread.Id, options);
    }

    /// <summary>
    /// Returns a collection of <see cref="ThreadRun"/> instances associated with an existing <see cref="AssistantThread"/>.
    /// </summary>
    /// <param name="thread"> The thread that runs in the list should be associated with. </param>
    /// <param name="options">Options describing the collection to return.</param>
    /// <returns></returns>
    public virtual PageCollection<ThreadRun> GetRuns(
        AssistantThread thread,
        RunCollectionOptions options = default)
    {
        Argument.AssertNotNull(thread, nameof(thread));

        return GetRuns(thread.Id, options);
    }
}
