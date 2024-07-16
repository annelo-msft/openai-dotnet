// <auto-generated/>

#nullable disable

using System;
using System.ComponentModel;

namespace OpenAI.FineTuning
{
    internal readonly partial struct InternalFineTuningIntegrationType : IEquatable<InternalFineTuningIntegrationType>
    {
        private readonly string _value;

        public InternalFineTuningIntegrationType(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        private const string WandbValue = "wandb";

        public static InternalFineTuningIntegrationType Wandb { get; } = new InternalFineTuningIntegrationType(WandbValue);
        public static bool operator ==(InternalFineTuningIntegrationType left, InternalFineTuningIntegrationType right) => left.Equals(right);
        public static bool operator !=(InternalFineTuningIntegrationType left, InternalFineTuningIntegrationType right) => !left.Equals(right);
        public static implicit operator InternalFineTuningIntegrationType(string value) => new InternalFineTuningIntegrationType(value);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is InternalFineTuningIntegrationType other && Equals(other);
        public bool Equals(InternalFineTuningIntegrationType other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(_value) : 0;
        public override string ToString() => _value;
    }
}
