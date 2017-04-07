using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CombatTest
{
    private const int HEALTH = 100;

    [TestMethod]
    public void TestTrivial()
    {
        var manager = new ArmyManager();
        var player = new Player(manager, new Color(0, 0, 0));
        var players = new List<Player>();
        players.Add(player);
        var combat = new CombatResolver(manager);
        combat.Engage(player, players);

        // We mostly care that this test terminated without error and effectively became almost a no-op.  For the sake of argument, we'll assert no new armies were created.
        Assert.AreEqual(0, player.ArmyList.Count);
    }

    [TestMethod]
    public void TestDoesNotAttackOwnArmies()
    {
        var manager = new ArmyManager();
        var player = new Player(manager, new Color(0, 0, 0));
        player.AddArmy(new Army(HEALTH), new Pos(0, 0));
        player.AddArmy(new Army(HEALTH), new Pos(1, 1));
        var players = new List<Player>();
        players.Add(player);
        players.Add(new Player(manager, new Color(1, 1, 1)));
        var combat = new CombatResolver(manager, 0);
        combat.Engage(player, players);

        Assert.AreEqual(2, player.ArmyList.Count);
        Assert.AreEqual(HEALTH, player.ArmyList[0].Health);
        Assert.AreEqual(HEALTH, player.ArmyList[1].Health);
    }

    [TestMethod]
    public void TestAttacksOtherArmies()
    {
        var manager = new ArmyManager();
        var attacking = new Player(manager, new Color(0, 0, 0));
        attacking.AddArmy(new Army(HEALTH), new Pos(0, 0));
        var defending = new Player(manager, new Color(1, 1, 1));
        defending.AddArmy(new Army(HEALTH), new Pos(1, 1));
        var players = new List<Player>();
        players.Add(attacking);
        players.Add(defending);
        var combat = new CombatResolver(manager, 0);
        combat.Engage(attacking, players);

        Assert.AreEqual(1, attacking.ArmyList.Count);
        Assert.AreEqual(1, defending.ArmyList.Count); // NOTE: The maximum damage inflicted < defender's health, so it should always survive.
        Assert.AreEqual(HEALTH, attacking.ArmyList[0].Health);
        Assert.IsTrue(defending.ArmyList[0].Health < HEALTH); // Not strictly true, but true for this random seed.
    }

    [TestMethod]
    public void TestAttacksAllOtherArmies()
    {
        var manager = new ArmyManager();
        var attacking = new Player(manager, new Color(0, 0, 0));
        attacking.AddArmy(new Army(HEALTH), new Pos(0, 0));
        var defending = new Player(manager, new Color(1, 1, 1));
        defending.AddArmy(new Army(HEALTH), new Pos(1, 1));
        defending.AddArmy(new Army(HEALTH), new Pos(2, 2));
        var defending2 = new Player(manager, new Color(2, 2, 2));
        defending2.AddArmy(new Army(HEALTH), new Pos(3, 3));
        var players = new List<Player>();
        players.Add(attacking);
        players.Add(defending);
        players.Add(defending2);
        var combat = new CombatResolver(manager, 0);
        combat.Engage(attacking, players);

        Assert.AreEqual(1, attacking.ArmyList.Count);
        Assert.AreEqual(2, defending.ArmyList.Count); // NOTE: The maximum damage inflicted < defender's health, so it should always survive.
        Assert.AreEqual(1, defending2.ArmyList.Count);
        Assert.AreEqual(HEALTH, attacking.ArmyList[0].Health);
        Assert.IsTrue(defending.ArmyList[0].Health < HEALTH); // Not strictly true, but true for this random seed.
        Assert.IsTrue(defending.ArmyList[1].Health < HEALTH);
        Assert.IsTrue(defending2.ArmyList[0].Health < HEALTH);
    }

    [TestMethod]
    public void TestArmiesDie()
    {
        var manager = new ArmyManager();
        var attacking = new Player(manager, new Color(0, 0, 0));
        attacking.AddArmy(new Army(HEALTH), new Pos(0, 0));
        var defending = new Player(manager, new Color(1, 1, 1));
        defending.AddArmy(new Army(0), new Pos(1, 1));
        var players = new List<Player>();
        players.Add(attacking);
        players.Add(defending);
        var combat = new CombatResolver(manager, 0);
        combat.Engage(attacking, players);

        Assert.AreEqual(1, attacking.ArmyList.Count);
        Assert.AreEqual(0, defending.ArmyList.Count);
        Assert.AreEqual(HEALTH, attacking.ArmyList[0].Health);
    }
}
