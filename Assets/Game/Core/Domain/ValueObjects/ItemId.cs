using System;

namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// ID của vật phẩm (strongly typed)
    /// </summary>
    public readonly struct ItemId : IEquatable<ItemId>
    {
        private readonly string _value;

        /// <summary>
        /// Lấy giá trị string của ItemId
        /// </summary>
        public string Value => _value;

        public ItemId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ItemId cannot be null or empty", nameof(value));
            _value = value;
        }

        public bool Equals(ItemId other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is ItemId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value?.GetHashCode() ?? 0;
        }

        public static bool operator ==(ItemId left, ItemId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ItemId left, ItemId right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return _value;
        }
    }
}

