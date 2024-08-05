// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;

namespace OpenAI.Embeddings
{
    public partial class EmbeddingTokenUsage
    {
        internal IDictionary<string, BinaryData> SerializedAdditionalRawData { get; set; }
        internal EmbeddingTokenUsage(int inputTokens, int totalTokens)
        {
            InputTokens = inputTokens;
            TotalTokens = totalTokens;
        }

        internal EmbeddingTokenUsage(int inputTokens, int totalTokens, IDictionary<string, BinaryData> serializedAdditionalRawData)
        {
            InputTokens = inputTokens;
            TotalTokens = totalTokens;
            SerializedAdditionalRawData = serializedAdditionalRawData;
        }

        internal EmbeddingTokenUsage()
        {
        }
        public int TotalTokens { get; }
    }
}
