using System;
using System.Collections.Generic;

public class Game
{
    private readonly World world;
    private int currentPlayer;
    private List<Player> players;

    public Game()
    {
        // initialize world
        this.world = new World("maps.csv");

        // set up players list. for now, just add two players.
        this.players = new List<Player>();
        this.players.Add(new Player());
        this.players.Add(new Player());
        this.currentPlayer = 0;
    }

    public static void Main(string[] args)
    {
        new Game().Launch();
    }

    public Player GetCurrentPlayer()
    {
        return this.players[this.currentPlayer];
    }

    public void AdvancePlayer()
    {
        this.currentPlayer = (this.currentPlayer + 1) % this.players.Count;
    }

    public void Launch()
    {
        Console.WriteLine("Welcome to WW3 - type 'quit' to exit.");
        string last;
        while ((last = Console.ReadLine()) != "quit")
        {
            if (last == "print")
            {
                this.Print();
            }
            else
            {
                Console.WriteLine("Command not recognized.");
            }
        }
    }

    public void Print()
    {
        for (int x = 0; x < World.WIDTH; x++)
        {
            for (int y = 0; y < World.HEIGHT; y++)
            {
                Province p = this.world.GetProvinceAt(new Pos(x, y));
                if (p.City != null)
                {
                    Console.Write(p.City.Name.Substring(0, 1));
                }
                else
                {
                    Console.Write("*");
                }
            }

            Console.WriteLine(string.Empty);
        }
    }
}