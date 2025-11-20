using System;

namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// Tiền trong game (immutable value object)
    /// </summary>
    public readonly struct Money : IComparable<Money>
    {
        private readonly long _amount;

        public long Amount => _amount;

        public Money(long amount)
        {
            if (amount < 0)
                throw new ArgumentException("Money cannot be negative", nameof(amount));
            _amount = amount;
        }

        public Money Add(Money other)
        {
            return new Money(_amount + other._amount);
        }

        public Money Subtract(Money other)
        {
            if (_amount < other._amount)
                throw new InvalidOperationException("Cannot subtract more money than available");
            return new Money(_amount - other._amount);
        }

        public bool IsGreaterThan(Money other)
        {
            return _amount > other._amount;
        }

        public bool IsLessThan(Money other)
        {
            return _amount < other._amount;
        }

        public bool IsGreaterThanOrEqual(Money other)
        {
            return _amount >= other._amount;
        }

        public bool IsLessThanOrEqual(Money other)
        {
            return _amount <= other._amount;
        }

        public int CompareTo(Money other)
        {
            return _amount.CompareTo(other._amount);
        }

        public static Money operator +(Money left, Money right)
        {
            return left.Add(right);
        }

        public static Money operator -(Money left, Money right)
        {
            return left.Subtract(right);
        }

        public static bool operator >(Money left, Money right)
        {
            return left.IsGreaterThan(right);
        }

        public static bool operator <(Money left, Money right)
        {
            return left.IsLessThan(right);
        }

        public static bool operator >=(Money left, Money right)
        {
            return left.IsGreaterThanOrEqual(right);
        }

        public static bool operator <=(Money left, Money right)
        {
            return left.IsLessThanOrEqual(right);
        }

        public override string ToString()
        {
            return _amount.ToString("N0");
        }
    }
}

