namespace Sandbox;

public readonly struct Point : IEquatable<Point>
{
    public readonly int X;
    public readonly int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public bool Equals(Point other)
    {
        return X == other.X && Y == other.Y;
    }
}