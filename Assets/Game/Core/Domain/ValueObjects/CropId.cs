using System;

namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// ID của cây trồng (strongly typed)
    /// </summary>
    public readonly struct CropId : IEquatable<CropId>
    {
        private readonly string _value;

        public CropId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("CropId cannot be null or empty", nameof(value));
            _value = value;
        }

        public bool Equals(CropId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is CropId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(CropId left, CropId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CropId left, CropId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}

