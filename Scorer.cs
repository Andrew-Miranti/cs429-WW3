using System.Collections.Generic;

public class Scorer
{
    private Dictionary<Player, int> points = new Dictionary<Player, int>();

    public void UpdateScores(World world)
    {
        points.Clear();

        for (int i = 0; i < World.WIDTH; i++)
        {
            for (int j = 0; j < World.HEIGHT; j++)
            {
                var p = world.GetProvinceAt(new Pos(i, j));
                if (p.Owner != null && p.City != null)
                {
                    if (points.ContainsKey(p.Owner))
                    {
                        points[p.Owner] += p.City.Points;
                    }
                    else
                    {
                        points.Add(p.Owner, p.City.Points);
                    }
                }
            }
        }
    }

    public int GetScore(Player player)
    {
        if (points.ContainsKey(player))
        {
            return points[player];
        }
        else
        {
            return 0;
        }
    }
}