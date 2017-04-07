using System;

public class Army
{
    public const int DefaultMoveRange = 20;
    public const int DefaultRange = 50;

    private readonly int maxHealth = 100;

    public Army(int health)
    {
        Health = health;
        Range = DefaultRange;
        MoveRange = DefaultMoveRange;
    }

    public bool Moved { get; set; }

    public int Range { get; private set; }

    public int MoveRange { get; private set; }

    public int Health { get; private set; }

    public void TakeDamage(int hurt)
    {
        Health = Math.Max(Health - hurt, 0);
    }

    public void FeedArmy(int food)
    {
        this.Health += food;
        this.Health = Math.Min(this.Health, maxHealth);
    }

    public void Tick()
    {
        Moved = false;
        /* Just restore Moved for now, may change into attack code in the future (i.e. armies attack enemies and capture territory at the end of their turn) */
    }
}
