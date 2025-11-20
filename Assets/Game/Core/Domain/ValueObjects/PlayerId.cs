using System;

namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// ID của người chơi (strongly typed)
    /// </summary>
    public readonly struct PlayerId : IEquatable<PlayerId>
    {
        private readonly string _value;

        public PlayerId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("PlayerId cannot be null or empty", nameof(value));
            _value = value;
        }

        public static PlayerId Default => new PlayerId("Player_Default");

        public bool Equals(PlayerId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(PlayerId left, PlayerId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerId left, PlayerId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}

