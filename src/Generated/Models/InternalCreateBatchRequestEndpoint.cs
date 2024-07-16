// <auto-generated/>

#nullable disable

using System;
using System.ComponentModel;

namespace OpenAI.Batch
{
    internal readonly partial struct InternalCreateBatchRequestEndpoint : IEquatable<InternalCreateBatchRequestEndpoint>
    {
        private readonly string _value;

        public InternalCreateBatchRequestEndpoint(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        private const string V1ChatCompletionsValue = "/v1/chat/completions";
        private const string V1EmbeddingsValue = "/v1/embeddings";

        public static InternalCreateBatchRequestEndpoint V1ChatCompletions { get; } = new InternalCreateBatchRequestEndpoint(V1ChatCompletionsValue);
        public static InternalCreateBatchRequestEndpoint V1Embeddings { get; } = new InternalCreateBatchRequestEndpoint(V1EmbeddingsValue);
        public static bool operator ==(InternalCreateBatchRequestEndpoint left, InternalCreateBatchRequestEndpoint right) => left.Equals(right);
        public static bool operator !=(InternalCreateBatchRequestEndpoint left, InternalCreateBatchRequestEndpoint right) => !left.Equals(right);
        public static implicit operator InternalCreateBatchRequestEndpoint(string value) => new InternalCreateBatchRequestEndpoint(value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is InternalCreateBatchRequestEndpoint other && Equals(other);
        public bool Equals(InternalCreateBatchRequestEndpoint other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        public override string ToString() => _value;
    }
}
