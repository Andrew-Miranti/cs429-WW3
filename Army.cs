public class Army
{
    public Army(Pos position, int health)
    {
        this.Position = position;
        this.Health = health;
    }

    public Pos Position { get; set; }

    public int Range { get; }

    public int Health { get; set; }
}