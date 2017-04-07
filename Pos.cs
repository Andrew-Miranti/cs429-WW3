using System;

public struct Pos
{
    public int X;

    public int Y;

    public Pos(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int DistanceTo(Pos target)
    {
        return Math.Abs(target.X - X) + Math.Abs(target.Y - Y);
    }

    public bool IsInRange(Pos target, int range)
    {
        return DistanceTo(target) <= range;
    }

    // override object.Equals
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Pos other = (Pos)obj;

        // TODO: write your implementation of Equals() here
        return other.X == X && other.Y == Y;
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "(" + X + ", " + Y + ")";
    }
}