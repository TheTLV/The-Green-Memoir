using System;

namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// ID của công cụ (strongly typed)
    /// </summary>
    public readonly struct ToolId : IEquatable<ToolId>
    {
        private readonly string _value;

        public ToolId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ToolId cannot be null or empty", nameof(value));
            _value = value;
        }

        public bool Equals(ToolId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is ToolId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(ToolId left, ToolId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ToolId left, ToolId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}

