using System;
using System.Collections.Generic;

public class Game
{
    private readonly World world;

    private List<Player> players;

    public Game()
    {
        // initialize World
        world = new World("maps.csv");

        // set up players list. for now, just add two players.
        players = new List<Player>();
        players.Add(new Player(new Color(1.0f, 0.0f, 0.0f)));
        players.Add(new Player(new Color(0.0f, 0.0f, 1.0f)));
        players[0].AddArmy(new Army(new Pos(0, 0), 10));
        players[1].AddArmy(new Army(new Pos(1, 1), 10));
        CurrentPlayerIndex = 0;
    }

    public Player CurrentPlayer
    {
        get
        {
            return players[CurrentPlayerIndex];
        }
    }

    public int CurrentPlayerIndex { get; private set; }

    public World World
    {
        get
        {
            return world;
        }
    }

    public void AdvancePlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Count;
    }

    public void Tick()
    {
        world.Tick();
        foreach (var player in players)
        {
            player.Tick();
        }
    }

    public void Print()
    {
        for (int x = 0; x < World.WIDTH; x++)
        {
            for (int y = 0; y < World.HEIGHT; y++)
            {
                Province p = world.GetProvinceAt(new Pos(x, y));
                bool printed = false;
                foreach (var player in players)
                {
                    var armies = player.ArmyList;
                    for (int j = 0; j < armies.Count; ++j)
                    {
                        if (armies[j].Position.Equals(new Pos(x, y)))
                        {
                            Console.Write(j);
                            printed = true;
                        }
                    }
                }

                if (!printed)
                {
                    Console.Write(p.City?.Name.Substring(0, 1) ?? "*");
                }
            }

            Console.WriteLine(string.Empty);
        }
    }

    public void Render()
    {
        world.Render();
        foreach (var player in players)
        {
            player.RenderArmies();
        }
    }
}