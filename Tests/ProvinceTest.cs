using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ProvinceTest
{
    [TestMethod]
    public void TestGatherWithCity()
    {
        Player owner = new Player(new ArmyManager(), new Color(0, 0, 0));
        Province p = new Province(new City("Urbana", 5), owner);
        p.Gather(owner);

        Assert.AreEqual(owner.Resources.GetAmountOf(ResourceType.Food), p.ActiveResources.GetAmountOf(ResourceType.Food));
        Assert.AreEqual(owner.Resources.GetAmountOf(ResourceType.Weapons), p.ActiveResources.GetAmountOf(ResourceType.Weapons));
    }

    [TestMethod]
    public void TestGatherNoCity()
    {
        Player owner = new Player(new ArmyManager(), new Color(0, 0, 0));
        Province p = new Province();
        p.Gather(owner);

        Assert.AreEqual(owner.Resources.GetAmountOf(ResourceType.Food), p.ActiveResources.GetAmountOf(ResourceType.Food));
        Assert.AreEqual(owner.Resources.GetAmountOf(ResourceType.Weapons), p.ActiveResources.GetAmountOf(ResourceType.Weapons));
    }

    [TestMethod]
    public void TestTickWithCity()
    {
        Player owner = new Player(new ArmyManager(), new Color(0, 0, 0));
        Province p = new Province(new City("Urbana", 5), owner);
        p.Tick();
        Assert.AreEqual(owner.Resources.GetAmountOf(ResourceType.Food), p.PassiveResources.GetAmountOf(ResourceType.Food));
        Assert.AreEqual(owner.Resources.GetAmountOf(ResourceType.Weapons), p.PassiveResources.GetAmountOf(ResourceType.Weapons));
    }

    [TestMethod]
    public void TestTickNoCity()
    {
        Player owner = new Player(new ArmyManager(), new Color(0, 0, 0));
        Province p = new Province(null, owner);
        p.Tick();
        Assert.AreEqual(owner.Resources.GetAmountOf(ResourceType.Food), p.PassiveResources.GetAmountOf(ResourceType.Food));
        Assert.AreEqual(owner.Resources.GetAmountOf(ResourceType.Weapons), p.PassiveResources.GetAmountOf(ResourceType.Weapons));
    }
}
