using System;
using Xunit;

namespace Tests
{
    public class GameTests
    {
        [Fact]
        public void TestPlayers()
        {
            Game test = new Game();
            Player one = test.GetCurrentPlayer();
            test.AdvancePlayer();
            Player two = test.GetCurrentPlayer();
            test.AdvancePlayer();
            Player three = test.GetCurrentPlayer();
            Assert.NotSame(one, two);
            Assert.Same(one, three);
        }
    }

    public class WorldTests
    {
        [Fact]
        public void TestParse()
        {
            World world = new World("maps.csv");
            Province prov = world.GetProvinceAt(new Pos(256, 128));
            Assert.NotNull(prov.City);
            Assert.Equal("Kabul", prov.City.Name);
            Assert.Equal(3160266, prov.City.Points);
        }
    }

    public class PlayerTests
    {
        public const int FULL_HEALTH = 100;
        [Fact]
        public void TestInitialEmpty()
        {
            Player test = new Player();
            Assert.Empty(test.ArmyList);
        }

        [Fact]
        public void TestAdd()
        {
            Player test = new Player();
            Pos source = new Pos(0, 0);
            Pos target = new Pos(2, 2);
            Army army = new Army(source, 2);
            test.AddArmy(army, target);
            Assert.NotEmpty(test.ArmyList);
            Army back = test.ArmyList[0];
            Assert.NotSame(army, back);
            Assert.Equal(army.Position, source);
            Assert.Equal(back.Position, target);
            Assert.Equal(back.Health, FULL_HEALTH);
        }

        [Fact]
        public void TestRemove()
        {
            Player test = new Player();
            Pos target = new Pos(2, 2);
            Army army = new Army(new Pos(0, 0), 2);
            test.AddArmy(army, target);
            Assert.NotEmpty(test.ArmyList);
            Army back = test.ArmyList[0];
            test.RemoveArmy(back);
            Assert.Empty(test.ArmyList);
        }

        [Fact]
        public void TestMove()
        {
            Player test = new Player();
            Pos start = new Pos(1, 1);
            Pos target = new Pos(2, 2);
            Army army = new Army(new Pos(0, 0), 2);
            test.AddArmy(army, new Pos(1, 1));
            Assert.NotEmpty(test.ArmyList);
            Army back = test.ArmyList[0];
            test.MoveArmy(back, target);
            Assert.Equal(back.Position, target);
        }
    }
}
