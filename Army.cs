using System;

public class Army
{
    public const int DefaultMoveRange = 20;
    public const int DefaultRange = 5;

    public const int MaxHealth = 100;

    private int health;

    public Army(int health)
    {
        Health = health;
        Range = DefaultRange;
        MoveRange = DefaultMoveRange;
    }

    public bool Moved { get; set; }

    public int Range { get; private set; }

    public int MoveRange { get; private set; }

    public int Health
    {
        get
        {
            return health;
        }

        private set
        {
            health = Math.Max(Math.Min(value, MaxHealth), 0);
        }
    }

    public void TakeDamage(int hurt)
    {
        Health -= hurt;
    }

    public void FeedArmy(int food)
    {
        Health += food;
    }

    public void Tick()
    {
        Moved = false;
        /* Just restore Moved for now, may change into attack code in the future (i.e. armies attack enemies and capture territory at the end of their turn) */
    }
}
