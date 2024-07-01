﻿using System;
using System.ClientModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

#nullable enable

namespace OpenAI.Assistants;

internal class MessageCollectionPageToken : OpenAIPageToken
{
    public MessageCollectionPageToken(string threadId, int? limit, string? order, string? after, string? before)
        : base(limit, order, after, before)
    {
        ThreadId = threadId;
    }

    public string ThreadId { get; }

    public override BinaryData ToBytes()
    {
        using MemoryStream stream = new();
        using Utf8JsonWriter writer = new(stream);

        writer.WriteStartObject();
        writer.WriteString("threadId", ThreadId);

        if (Limit.HasValue)
        {
            writer.WriteNumber("limit", Limit.Value);
        }

        if (Order is not null)
        {
            writer.WriteString("order", Order);
        }

        if (After is not null)
        {
            writer.WriteString("after", After);
        }

        if (Before is not null)
        {
            writer.WriteString("before", Before);
        }

        writer.WriteEndObject();

        writer.Flush();
        stream.Position = 0;

        return BinaryData.FromStream(stream);
    }
    
    public override OpenAIPageToken? GetNextPageToken(bool hasMore, string? lastId)
         => GetNextPageToken(ThreadId, Limit, Order, lastId, Before, hasMore);

    // Convenience - first page request
    public static MessageCollectionPageToken FromOptions(string threadId, MessageCollectionOptions options)
        => new(threadId, options?.PageSize, options?.Order?.ToString(), options?.AfterId, options?.BeforeId);

    // Convenience - continuation page request
    public static MessageCollectionPageToken FromToken(ContinuationToken pageToken)
    {
        if (pageToken is MessageCollectionPageToken token)
        {
            return token;
        }

        BinaryData data = pageToken.ToBytes();

        if (data.ToMemory().Length == 0)
        {
            return new(string.Empty, default, default, default, default);
        }

        Utf8JsonReader reader = new(data);

        string threadId = null!;
        int? limit = null;
        string? order = null;
        string? after = null;
        string? before = null;

        reader.Read();
        Debug.Assert(reader.TokenType == JsonTokenType.StartObject);

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);
            string propertyName = reader.GetString()!;

            switch (propertyName)
            {
                case "threadId":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    threadId = reader.GetString()!;
                    break;
                case "limit":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.Number);
                    limit = reader.GetInt32();
                    break;
                case "order":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    order = reader.GetString();
                    break;
                case "after":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    after = reader.GetString();
                    break;
                case "before":
                    reader.Read();
                    Debug.Assert(reader.TokenType == JsonTokenType.String);
                    before = reader.GetString();
                    break;
                default:
                    throw new JsonException($"Unrecognized property '{propertyName}'.");
            }
        }

        if (threadId is null)
        {
            throw new ArgumentException("Failed to create MessageCollectionPageToken from provided pageToken.", nameof(pageToken));
        }

        return new(threadId, limit, order, after, before);
    }

    // Protocol
    public static MessageCollectionPageToken FromOptions(string threadId, int? limit, string? order, string? after, string? before)
        => new MessageCollectionPageToken(threadId, limit, order, after, before);

    private static MessageCollectionPageToken? GetNextPageToken(string threadId, int? limit, string? order, string? after, string? before, bool hasMore)
    {
        if (!hasMore || after is null)
        {
            return null;
        }

        return new MessageCollectionPageToken(threadId, limit, order, after, before);
    }
}