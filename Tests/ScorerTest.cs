using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ScorerTest
{
    private const string MAPSFILE = "maps.csv";

    [TestMethod]
    public void TestNoPlayers()
    {
        var scorer = new Scorer();
        scorer.UpdateScores(new World(MAPSFILE));

        // No actual assertions.  The only expected behavior here is that this test must complete without error.
    }

    [TestMethod]
    public void TestSinglePlayerNoneOwned()
    {
        var player = new Player(new ArmyManager(), new Color(0, 0, 0));
        var players = new List<Player>();
        players.Add(player);
        var scorer = new Scorer();
        scorer.UpdateScores(new World(MAPSFILE));

        Assert.AreEqual(0, scorer.GetScore(player));
    }

    [TestMethod]
    public void TestSinglePlayerAllOwned()
    {
        var player = new Player(new ArmyManager(), new Color(0, 0, 0));
        var players = new List<Player>();
        players.Add(player);
        var scorer = new Scorer();
        var world = new World(MAPSFILE);
        int sum = 0;
        for (int i = 0; i < World.WIDTH; i++)
        {
            for (int j = 0; j < World.HEIGHT; j++)
            {
                Province p = world.GetProvinceAt(new Pos(i, j));
                p.Owner = player;
                if (p.City != null)
                {
                    sum += p.City.Points;
                }
            }
        }

        scorer.UpdateScores(world);
        Assert.AreEqual(sum, scorer.GetScore(player));
    }

    [TestMethod]
    public void TestTwoPlayerAllOwnedOne()
    {
        var player = new Player(new ArmyManager(), new Color(0, 0, 0));
        var player2 = new Player(new ArmyManager(), new Color(0, 0, 0));
        var players = new List<Player>();
        players.Add(player);
        players.Add(player2);
        var scorer = new Scorer();
        var world = new World(MAPSFILE);
        int sum = 0;
        for (int i = 0; i < World.WIDTH; i++)
        {
            for (int j = 0; j < World.HEIGHT; j++)
            {
                Province p = world.GetProvinceAt(new Pos(i, j));
                p.Owner = player;
                if (p.City != null)
                {
                    sum += p.City.Points;
                }
            }
        }

        scorer.UpdateScores(world);

        Assert.AreEqual(sum, scorer.GetScore(player));
        Assert.AreEqual(0, scorer.GetScore(player2));
    }

    [TestMethod]
    public void TestParameterizedOnePlayer()
    {
        Random random = new Random();
        int[] scores = { -5, 4, 0, 20, 1000000, random.Next() };
        foreach (int score in scores)
        {
            TestParameterizedOnePlayer(score);
        }
    }

    public void TestParameterizedOnePlayer(int num)
    {
        var player = new Player(new ArmyManager(), new Color(0, 0, 0));
        var players = new List<Player>();
        players.Add(player);
        var scorer = new Scorer();
        var world = new World();
        var prov = world.GetProvinceAt(new Pos(0, 0));
        prov.City = new City("Urbana", num);
        prov.Owner = player;
        scorer.UpdateScores(world);

        Assert.AreEqual(num, scorer.GetScore(player));
    }
}