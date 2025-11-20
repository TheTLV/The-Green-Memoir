using System;

namespace TheGreenMemoir.Core.Domain.ValueObjects
{
    /// <summary>
    /// Vị trí ô đất trong tilemap (immutable value object)
    /// </summary>
    public readonly struct TilePosition : IEquatable<TilePosition>
    {
        public int X { get; }
        public int Y { get; }

        public TilePosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(TilePosition other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is TilePosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(TilePosition left, TilePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TilePosition left, TilePosition right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"Tile({X}, {Y})";
        }
    }
}

