using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GameTest
{
    private const int SEED = 1;

    [TestMethod]
    public void TestPlayerRotation()
    {
        var test = Game.DefaultGame();
        var one = test.CurrentPlayer;
        test.AdvancePlayer();
        var two = test.CurrentPlayer;
        test.AdvancePlayer();
        var three = test.CurrentPlayer;
        Assert.AreNotSame(one, two);
        Assert.AreSame(one, three);
    }

    [TestMethod]
    public void TestSavesTicks()
    {
        Random random = new Random(SEED);
        for (int i = 0; i < 10; ++i)
        {
            TestSavesTicksParam(random.Next(Game.MAXTICKS));
        }
    }

    public void TestSavesTicksParam(int turns)
    {
        var test = Game.DefaultGame();
        for (int i = 0; i < turns; ++i)
        {
            test.EndTurn();
        }

        string data = test.SerializeJSON();
        Assert.IsTrue(data.Contains("\"ticks\": " + turns));
    }

    [TestMethod]
    public void TestSavesComplete()
    {
        var test = Game.DefaultGame();
        for (int i = 0; i < Game.MAXTICKS + 1; i++)
        {
            test.EndTurn();
        }

        string data = test.SerializeJSON();
        Assert.IsTrue(test.Finished);
        Assert.IsTrue(data.Contains("\"finished\": true"));
    }

    [TestMethod]
    public void TestSavesTurn()
    {
        var test = Game.DefaultGame();
        var turnOne = test.SerializeJSON();
        test.EndTurn();
        var turnTwo = test.SerializeJSON();
        test.EndTurn();
        var turnThree = test.SerializeJSON();

        Assert.IsTrue(turnOne.Contains("\"current\": 0"));
        Assert.IsTrue(turnTwo.Contains("\"current\": 1"));
        Assert.IsTrue(turnThree.Contains("\"current\": 0"));
    }

    [TestMethod]
    public void TestSavesPlayer()
    {
        // Counts number of players by counting occurances of color.
        const string PLAYERPROXY = "\"color\": ";
        var test = Game.DefaultGame();
        var data = test.SerializeJSON();
        var firstIndex = data.IndexOf(PLAYERPROXY);
        var lastIndex = data.LastIndexOf(PLAYERPROXY);
        Assert.IsTrue(firstIndex != lastIndex);
    }

    [TestMethod]
    public void TestSavesArmy()
    {
        // Counts number of armies by counting occurances of origin.
        const string PLAYERPROXY = "\"origin\": ";
        var test = Game.DefaultGame();
        var data = test.SerializeJSON();
        var firstIndex = data.IndexOf(PLAYERPROXY);
        var lastIndex = data.LastIndexOf(PLAYERPROXY);
        Assert.IsTrue(firstIndex > 0);
        Assert.IsTrue(lastIndex > 0);
        Assert.IsTrue(firstIndex != lastIndex);
    }

    [TestMethod]
    public void TestRestoresResources()
    {
        var random = new Random(SEED);
        for (int i = 0; i < 10; i++)
        {
            TestRestoresResourcesParam(random.Next());
        }
    }

    public void TestRestoresResourcesParam(int food)
    {
        var test = Game.DefaultGame();
        test.Players[0].Resources.SetAmountOf(ResourceType.Food, food);
        var data = test.SerializeJSON();
        var back = Game.FromJSON(data);
        Assert.AreEqual(food, test.Players[0].Resources.GetAmountOf(ResourceType.Food));
    }

    [TestMethod]
    public void TestRestoresArmies()
    {
        var random = new Random(SEED);
        for (int i = 0; i < 10; i++)
        {
            TestRestoresArmiesParam(random.Next(Army.MaxHealth), new Pos(random.Next(World.WIDTH), random.Next(World.HEIGHT)));
        }
    }

    public void TestRestoresArmiesParam(int health, Pos pos)
    {
        var test = Game.DefaultGame();
        test.Players[0].AddArmy(new Army(health), pos);
        var data = test.SerializeJSON();
        var back = Game.FromJSON(data);
        var armyBack = back.Manager.ArmyAt(pos);
        Assert.IsNotNull(armyBack);
        Assert.AreEqual(health, armyBack.Health);
    }

    [TestMethod]
    public void TestRestoresFinished()
    {
        var test = Game.DefaultGame();
        var backOne = Game.FromJSON(test.SerializeJSON());
        for (int i = 0; i < Game.MAXTICKS + 1; i++)
        {
            test.EndTurn();
        }

        var backTwo = Game.FromJSON(test.SerializeJSON());
        Assert.IsTrue(test.Finished);
        Assert.IsFalse(backOne.Finished);
        Assert.IsTrue(backTwo.Finished);
    }

    [TestMethod]
    public void TestRestoresManyArmies()
    {
        const int MAXARMIES = 90;
        var random = new Random(SEED);
        for (int i = 0; i < 10; i++)
        {
            TestRestoresManyArmiesParam(random.Next(MAXARMIES));
        }
    }

    public void TestRestoresManyArmiesParam(int num)
    {
        var test = Game.DefaultGame();
        test.Players[0].RemoveArmy(test.Players[0].GetArmy(0));
        test.Players[1].RemoveArmy(test.Players[1].GetArmy(0));

        for (int i = 0; i < num; i++)
        {
            test.Players[0].AddArmy(new Army(i + 1), new Pos(i, i));
            test.Players[1].AddArmy(new Army(i + 1), new Pos(i + 1, i));
        }

        var back = Game.FromJSON(test.SerializeJSON());
        for (int i = 0; i < num; i++)
        {
            var armyOne = back.Players[0].GetArmy(i);
            Assert.IsNotNull(armyOne);
            Assert.AreEqual(i + 1, armyOne.Health);
            Assert.AreEqual(new Pos(i, i), back.Manager.ArmyPosition(armyOne));
            var armyTwo = back.Players[1].GetArmy(i);
            Assert.IsNotNull(armyTwo);
            Assert.AreEqual(i + 1, armyTwo.Health);
            Assert.AreEqual(new Pos(i + 1, i), back.Manager.ArmyPosition(armyTwo));
        }
    }

    [TestMethod]
    public void TestRestoresColors()
    {
        const int MAXCOLOR = 255;
        var random = new Random(SEED);
        for (int i = 0; i < 10; i++)
        {
            TestRestoresColorsParam(random.Next(MAXCOLOR), random.Next(MAXCOLOR), random.Next(MAXCOLOR));
        }
    }

    public void TestRestoresColorsParam(int r, int g, int b)
    {
        var test = Game.DefaultGame();
        test.Players[0].Color = new Color(r, g, b);
        var back = Game.FromJSON(test.SerializeJSON());
        var color = back.Players[0].Color;
        Assert.AreEqual(r, color.R);
        Assert.AreEqual(g, color.G);
        Assert.AreEqual(b, color.B);
    }

    [TestMethod]
    public void TestNoWinner()
    {
        var test = Game.DefaultGame();
        Assert.IsFalse(test.Finished);
    }

    [TestMethod]
    public void TestFinishesAfterTurns()
    {
        var test = Game.DefaultGame();
        for (int i = 0; i < Game.MAXTICKS + 1; i++)
        {
            test.EndTurn();
        }

        Assert.IsTrue(test.Finished);
    }

    [TestMethod]
    public void TestTurnCounts()
    {
        var random = new Random(SEED);
        for (int i = 0; i < 10; i++)
        {
            TestTurnCountsParam(random.Next(Game.MAXTICKS));
        }
    }

    public void TestTurnCountsParam(int turns)
    {
        var test = Game.DefaultGame();
        for (int i = 0; i < turns; i++)
        {
            test.EndTurn();
        }

        Assert.AreEqual(turns, test.Ticks);
    }

    [TestMethod]
    public void TestGameEnds()
    {
        var test = Game.DefaultGame();
        for (int i = 0; i < Game.MAXTICKS * 2; i++)
        {
            test.EndTurn();
        }

        Assert.AreEqual(Game.MAXTICKS, test.Ticks);
    }

    [TestMethod]
    public void TestWinner()
    {
        var test = Game.DefaultGame();

        for (int x = 0; x < World.WIDTH; x++)
        {
            for (int y = 0; y < World.HEIGHT; y++)
            {
                test.GameWorld.GetProvinceAt(new Pos(x, y)).Owner = test.Players[0];
            }
        }

        for (int i = 0; i < Game.MAXTICKS + 1; i++)
        {
            test.EndTurn();
        }

        Assert.IsTrue(test.Finished);
        Assert.AreEqual(0, test.WinCondition());
    }
}