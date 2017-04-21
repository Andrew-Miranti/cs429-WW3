using System.Collections.Generic;

public class Game
{
    private const int MAXTICKS = 200;

    private Scorer scorer;

    private CombatResolver combat;

    private int ticks = 0;

    public Game()
    {
        World = new World("maps.csv");
        Manager = new ArmyManager();
        combat = new CombatResolver(Manager);
        Players = new List<Player>();
        Players.Add(new Player(Manager, new Color(1.0f, 0.0f, 0.0f)));
        Players.Add(new Player(Manager, new Color(0.0f, 0.0f, 1.0f)));
        Players[0].AddArmy(new Army(10), new Pos(0, 0));
        Players[1].AddArmy(new Army(10), new Pos(1, 1));
        CurrentPlayerIndex = 0;
        scorer = new Scorer(Players);
    }

    public World World { get; private set; }

    public List<Player> Players { get; private set; }

    public ArmyManager Manager { get; private set; }

    public Player CurrentPlayer
    {
        get
        {
            return Players[CurrentPlayerIndex];
        }
    }

    public int CurrentPlayerIndex { get; private set; }

    public void AdvancePlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
    }

    public void Tick()
    {
        World.Tick();
        Manager.Tick();
        foreach (var player in Players)
        {
            player.Tick();
        }

        combat.Engage(CurrentPlayer, Players);
        scorer.UpdateScores(World);
        ticks += 1;
    }

    public void EndTurn()
    {
        Tick();
        AdvancePlayer();
    }

    public int ScorePlayer(Player player)
    {
        return scorer.GetScore(player);
    }

    public int WinCondition()
    {
        int winner = -1;

        if (ticks >= MAXTICKS)
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
