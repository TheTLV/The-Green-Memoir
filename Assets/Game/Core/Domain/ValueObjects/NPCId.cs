using System;

namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// ID của NPC (strongly typed)
    /// </summary>
    public readonly struct NPCId : IEquatable<NPCId>
    {
        private readonly string _value;

        public NPCId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("NPCId cannot be null or empty", nameof(value));
            _value = value;
        }

        public static NPCId Default => new NPCId("NPC_Default");

        public bool Equals(NPCId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is NPCId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(NPCId left, NPCId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NPCId left, NPCId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}

