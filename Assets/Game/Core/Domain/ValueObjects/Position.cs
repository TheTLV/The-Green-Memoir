using System;

namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// Vị trí trong game (immutable value object)
    /// </summary>
    public readonly struct Position : IEquatable<Position>
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Tính khoảng cách đến vị trí khác
        /// </summary>
        public float DistanceTo(Position other)
        {
            int dx = X - other.X;
            int dy = Y - other.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Kiểm tra xem có phải ô kề nhau không
        /// </summary>
        public bool IsAdjacentTo(Position other)
        {
            int dx = Math.Abs(X - other.X);
            int dy = Math.Abs(Y - other.Y);
            return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
        }

        public bool Equals(Position other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Position other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}

