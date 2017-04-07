using System.Collections.Generic;

public class Scorer
{
    private Dictionary<Player, int> points = new Dictionary<Player, int>();

    public Scorer(List<Player> players)
    {
        foreach (var player in players)
        {
            points.Add(player, 0);
        }
    }

    public void UpdateScores(World world)
    {
        foreach (var player in new List<Player>(points.Keys))
        {
            points[player] = 0;
        }

        for (int i = 0; i < World.WIDTH; i++)
        {
            for (int j = 0; j < World.HEIGHT; j++)
            {
                Province p = world.GetProvinceAt(new Pos(i, j));
                if (p.Owner != null && p.City != null)
                {
                    points[p.Owner] += p.City.Points;
                }
            }
        }
    }

    public int GetScore(Player player)
    {
        return points[player];
    }
}