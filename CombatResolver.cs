using System;
using System.Collections.Generic;

public class CombatResolver
{
    private const int DAMAGE = 10;

    private Random generator;

    private ArmyManager armies;

    public CombatResolver(ArmyManager armies)
    {
        this.armies = armies;
        generator = new Random();
    }

    public CombatResolver(ArmyManager armies, int seed)
    {
        this.armies = armies;
        generator = new Random(seed);
    }

    public void Engage(Player moving, List<Player> players)
    {
        foreach (var player in players)
        {
            if (player != moving)
            {
                AttackPlayer(moving, player);
            }
        }
    }

    private void AttackPlayer(Player moving, Player player)
    {
        foreach (var attacking in moving.ArmyList)
        {
            foreach (var defending in player.ArmyList)
            {
                if (armies.ArmyPosition(attacking).IsInRange(armies.ArmyPosition(defending), attacking.Range))
                {
                    defending.TakeDamage(generator.Next(1, DAMAGE));
                }
            }
        }

        for (int i = 0; i < player.ArmyList.Count; i++)
        {
            var army = player.ArmyList[i];
            if (army.Health <= 0)
            {
                player.ArmyList.RemoveAt(i);
                i--;
            }
        }
    }
}