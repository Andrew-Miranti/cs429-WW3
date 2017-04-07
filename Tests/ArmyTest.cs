using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ArmyTest
{
    [TestMethod]
    public void TestConstruction()
    {
        var a = new Army(100);
        Assert.AreEqual(100, a.Health);
    }
}
