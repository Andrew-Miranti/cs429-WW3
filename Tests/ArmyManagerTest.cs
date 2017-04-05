using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ArmyManagerTest
{
    private const int SEED = 1;
    private const int NUM = 10;
    private const int MAX = 500;

    [TestMethod]
    public void TestInitialAdd()
    {
        var manager = new ArmyManager();
        var player = new Player(manager, new Color(0, 0, 0));
        player.AddArmy(new Army(100), new Pos(1, 1));

        Assert.AreEqual(1, player.ArmyList.Count);
        Assert.AreEqual(new Pos(1, 1), manager.ArmyPosition(player.ArmyList[0]));
        Assert.AreEqual(player.ArmyList[0], manager.ArmyAt(new Pos(1, 1)));
    }

    [TestMethod]
    public void TestMovement()
    {
        Random random = new Random(SEED);
        for (int i = 0; i < NUM; i++)
        {
            var source = GenerateRandom(random);
            TestMovementParam(source, GenerateRandomInRange(source, Army.DefaultMoveRange, random));
        }
    }

    public void TestMovementParam(Pos source, Pos dest)
    {
        var manager = new ArmyManager();
        var player = new Player(manager, new Color(0, 0, 0));
        var army = new Army(100);
        player.AddArmy(army, source);
        manager.MoveArmy(army, dest);

        Assert.IsTrue((manager.ArmyAt(source) != null) == source.Equals(dest)); // source != dest implies that army will no longer be at source.
        Assert.AreSame(army, manager.ArmyAt(dest));
        Assert.AreEqual(dest, manager.ArmyPosition(army));
    }

    [TestMethod]
    public void TestIllegalMovement()
    {
        Random random = new Random(SEED);
        for (int i = 0; i < NUM; i++)
        {
            var source = GenerateRandom(random);
            var dest = new Pos(source.X + Army.DefaultMoveRange, source.Y + Army.DefaultMoveRange);
            TestIllegalMovementParam(source, dest);
        }
    }

    public void TestIllegalMovementParam(Pos source, Pos dest)
    {
        var manager = new ArmyManager();
        var player = new Player(manager, new Color(0, 0, 0));
        var army = new Army(100);
        player.AddArmy(army, source);
        manager.MoveArmy(army, dest);

        Assert.IsNull(manager.ArmyAt(dest));
        Assert.AreSame(army, manager.ArmyAt(source));
        Assert.AreEqual(source, manager.ArmyPosition(army));
    }

    [TestMethod]
    public void TestUndoMove()
    {
        Random random = new Random(SEED);
        for (int i = 0; i < NUM; i++)
        {
            var source = GenerateRandom(random);
            TestUndoParam(source, GenerateRandomInRange(source, Army.DefaultMoveRange, random));
        }
    }

    public void TestUndoParam(Pos source, Pos dest)
    {
        var manager = new ArmyManager();
        var player = new Player(manager, new Color(0, 0, 0));
        var army = new Army(100);
        player.AddArmy(army, source);
        manager.MoveArmy(army, dest);
        manager.UndoMove(army);

        Assert.AreSame(army, manager.ArmyAt(source));
        Assert.AreEqual(source, manager.ArmyPosition(army));
    }

    [TestMethod]
    public void TestUndoCycle() // Tests a particularly insidious case of UndoMove wherein one Undo forces another in a cycle.
    {
        const int NumArmies = 5;
        var manager = new ArmyManager();
        var player = new Player(manager, new Color(0, 0, 0));
        var armies = new List<Army>();
        for (int i = 0; i < NumArmies; i++)
        {
            var army = new Army(100);
            armies.Add(army);
            player.AddArmy(army, new Pos(i, i));
        }

        for (int i = NumArmies - 1; i >= 0; i--)
        {
            manager.MoveArmy(armies[i], new Pos(i + 1, i + 1));
        }

        manager.MoveArmy(armies[NumArmies - 1], new Pos(0, 0));
        manager.UndoMove(armies[NumArmies - 1]);
        for (int i = 0; i < NumArmies; i++)
        {
            Assert.AreEqual(new Pos(i, i), manager.ArmyPosition(armies[i]));
        }
    }

    private Pos GenerateRandom(Random generator)
    {
        return new Pos(generator.Next(0, MAX), generator.Next(0, MAX));
    }

    private Pos GenerateRandomInRange(Pos source, int range, Random generator)
    {
        return new Pos(source.X + generator.Next(-range / 2, range / 2), source.Y + generator.Next(-range / 2, range / 2));
    }
}