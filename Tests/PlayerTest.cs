using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PlayerTest
{
    // public const int FullHealth = 100;

    // [TestMethod]
    // public void TestInitialEmpty()
    // {
    //     Player test = new Player();
    //     Assert.IsFalse(test.ArmyList.Any());
    // }

    // [TestMethod]
    // public void TestAdd()
    // {
    //     Player test = new Player();
    //     Pos source = new Pos(0, 0);
    //     Pos target = new Pos(2, 2);
    //     Army army = new Army(source, 2);
    //     test.AddArmy(army);
    //     Assert.IsTrue(test.ArmyList.Any());
    //     Army back = test.ArmyList[0];
    //     Assert.AreNotSame(army, back);
    //     Assert.AreEqual(army.Position, source);
    //     Assert.AreEqual(back.Position, target);
    //     Assert.AreEqual(back.Health, FullHealth);
    // }

    // [TestMethod]
    // public void TestRemove()
    // {
    //     Player test = new Player();
    //     Pos target = new Pos(2, 2);
    //     Army army = new Army(new Pos(0, 0), 2);
    //     test.AddArmy(army);
    //     Assert.IsTrue(test.ArmyList.Any());
    //     Army back = test.ArmyList[0];
    //     test.RemoveArmy(back);
    //     Assert.IsFalse(test.ArmyList.Any());
    // }

    // [TestMethod]
    // public void TestMove()
    // {
    //     Player test = new Player();
    //     Pos start = new Pos(1, 1);
    //     Pos target = new Pos(2, 2);
    //     Army army = new Army(start, 2);
    //     test.AddArmy(army);
    //     Assert.IsTrue(test.ArmyList.Any());
    //     Army back = test.ArmyList[0];
    //     test.MoveArmy(back, target);
    //     Assert.AreEqual(back.Position, target);
    // }
}