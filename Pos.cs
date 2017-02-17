public struct Pos
{
    public int X;

    public int Y;

    public Pos(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public override string ToString()
    {
        return "(" + this.X + ", " + this.Y + ")";
    }
}