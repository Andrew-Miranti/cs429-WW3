using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class WorldTest
{
    private const string FILE = "./../../../maps.csv";

    [TestMethod]
    public void TestParseWorldCSV()
    {
        World world = new World(FILE);
        Province prov = world.GetProvinceAt(new Pos(29, 10));
        Assert.IsNotNull(prov.City);
        Assert.AreEqual("Puerto Natales", prov.City.Name);
        Assert.AreEqual(10393, prov.City.Points);
    }
}