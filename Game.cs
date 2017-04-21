using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

public class Game
{
    public const int MAXTICKS = 200;

    private Scorer scorer;

    private CombatResolver combat;

    public Game()
    {
        GameWorld = new World("maps.csv");
        Manager = new ArmyManager();
        scorer = new Scorer();
        combat = new CombatResolver(Manager);
        Players = new List<Player>();
        CurrentPlayerIndex = 0;
    }

    public World GameWorld { get; private set; }

    public List<Player> Players { get; private set; }

    public ArmyManager Manager { get; private set; }

    public bool Finished { get; private set; }

    public int Ticks { get; private set; }

    public Player CurrentPlayer
    {
        get
        {
            return Players[CurrentPlayerIndex];
        }
    }

    public int CurrentPlayerIndex { get; private set; }

    public static Game LoadFromFile(string filename)
    {
        return FromJSON(File.ReadAllText(filename));
    }

    public static Game FromJSON(string input)
    {
        var data = JObject.Parse(input);
        var result = new Game();
        result.CurrentPlayerIndex = data.GetValue("current").ToObject<int>();
        result.Ticks = data.GetValue("ticks").ToObject<int>();

        result.Finished = data.GetValue("finished").ToObject<bool>();
        foreach (JObject playerJSON in (JArray)data.GetValue("players"))
        {
            var player = new Player(result.Manager, playerJSON.GetValue("color").ToObject<Color>());
            player.Resources.Add(playerJSON.GetValue("resources").ToObject<ResourceBag>());
            foreach (JObject armyJSON in (JArray)playerJSON.GetValue("armies"))
            {
                var army = armyJSON.GetValue("army").ToObject<Army>();
                player.AddArmy(army, armyJSON.GetValue("origin").ToObject<Pos>());
                var dest = armyJSON.GetValue("destination");
                if (dest != null)
                {
                    result.Manager.MoveArmy(army, dest.ToObject<Pos>());
                }
            }

            result.Players.Add(player);
        }

        var ownersArray = (JArray)data.GetValue("owners");

        for (int x = 0; x < World.WIDTH; x++)
        {
            for (int y = 0; y < World.HEIGHT; y++)
            {
                var playerNumber = ownersArray[(x * World.HEIGHT) + y].ToObject<int>();
                if (playerNumber == -1)
                {
                    result.GameWorld.GetProvinceAt(new Pos(x, y)).Owner = null;
                }
                else
                {
                    result.GameWorld.GetProvinceAt(new Pos(x, y)).Owner = result.Players[playerNumber];
                }
            }
        }

        return result;
    }

    public static Game DefaultGame()
    {
        var result = new Game();
        result.Players.Add(new Player(result.Manager, new Color(0, 0, 0)));
        result.Players.Add(new Player(result.Manager, new Color(1, 1, 1)));
        result.Players[0].AddArmy(new Army(100), new Pos(0, 0));
        result.Players[1].AddArmy(new Army(100), new Pos(1, 1));
        return result;
    }

    public void AdvancePlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
    }

    public void Tick()
    {
        GameWorld.Tick();
        Manager.Tick();
        foreach (var player in Players)
        {
            player.Tick();
        }

        combat.Engage(CurrentPlayer, Players);

        foreach (var player in Players)
        {
            foreach (var army in player.ArmyList)
            {
                if (army.Health > 0)
                {
                    GameWorld.GetProvinceAt(Manager.ArmyPosition(army)).Owner = player;
                }
            }
        }

        scorer.UpdateScores(GameWorld);
        Ticks += 1;
    }

    public void EndTurn()
    {
        if (WinCondition() < 0)
        {
            Tick();
            AdvancePlayer();
        }
        else
        {
            Finished = true;
        }
    }

    public int ScorePlayer(Player player)
    {
        return scorer.GetScore(player);
    }

    public string SerializeJSON()
    {
        var root = new JObject();
        var playersArray = new JArray();
        var ownersArray = new JArray();
        root.Add("ticks", Ticks);
        root.Add("finished", Finished);
        root.Add("players", playersArray);
        root.Add("owners", ownersArray);
        root.Add("current", new JValue(CurrentPlayerIndex));
        root.Add("finished", new JValue(Finished));

        foreach (var player in Players)
        {
            var playerObject = new JObject();
            var armyArray = new JArray();
            playerObject.Add("color", JObject.FromObject(player.Color));
            playerObject.Add("resources", JObject.FromObject(player.Resources));
            playerObject.Add("armies", armyArray);
            foreach (var army in player.ArmyList)
            {
                var positionedArmy = new JObject();
                positionedArmy.Add("army", JObject.FromObject(army));
                positionedArmy.Add("origin", JObject.FromObject(Manager.ArmyStartingPosition(army)));
                if (Manager.HasArmyMoved(army))
                {
                    positionedArmy.Add("destination", JObject.FromObject(Manager.ArmyPosition(army)));
                }

                armyArray.Add(positionedArmy);
            }

            playersArray.Add(playerObject);
        }

        for (int x = 0; x < World.WIDTH; x++)
        {
            for (int y = 0; y < World.HEIGHT; y++)
            {
                ownersArray.Add(new JValue(Players.IndexOf(GameWorld.GetProvinceAt(new Pos(x, y)).Owner)));
            }
        }

        return root.ToString();
    }

    public void SaveToFile()
    {
        const string PREFIX = "ww3";
        const string SUFFIX = ".json";
        var data = SerializeJSON();
        var name = PREFIX + System.DateTime.Now.Ticks + SUFFIX;
        File.WriteAllText(name, data);
    }

    public int WinCondition()
    {
        int winner = -1;

        if (Ticks >= MAXTICKS)
        {
            var maxScore = int.MinValue;

            for (int i = 0; i < Players.Count; i++)
            {
                var score = scorer.GetScore(Players[i]);
                if (score > maxScore)
                {
                    maxScore = score;
                    winner = i;
                }
            }
        }

        return winner;
    }
}
