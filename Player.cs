using System.Collections.Generic;

public class Player
{
    private Dictionary<Army, Pos?> originals;

    public Player(Color c)
    {
        ArmyList = new List<Army>();
        originals = new Dictionary<Army, Pos?>();
        Color = c;
        Resources = new ResourceBag();
    }

    public List<Army> ArmyList { get; private set; }

    public Color Color { get; set; }

    public ResourceBag Resources { get; private set; }

    public void AddArmy(Army army)
    {
        // create new army object
        ArmyList.Add(army);
    }

    public Army GetArmy(int id)
    {
        if (ArmyExists(id))
        {
            return ArmyList[id];
        }

        return null;
    }

    public bool ArmyExists(int id)
    {
        return id >= 0 && id < ArmyList.Count;
    }

    public bool FeedArmy(int armyId, int foodAmount)
    {
        if (!Resources.Use(ResourceType.Food, foodAmount) || armyId >= ArmyList.Count)
        {
            return false;
        }

        ArmyList[armyId].FeedArmy(foodAmount);
        return true;
    }

    public Pos ArmyPosition(int armyId)
    {
        return ArmyList[armyId].Position;
    }

    public bool CanMoveArmy(int armyId, Pos toPos)
    {
        if (!ArmyExists(armyId))
        {
            return false;
        }

        var army = GetArmy(armyId);
        Pos originalPosition = UnmovedPosition(army);
        Pos startingPosition = army.Position;
        army.Position = originalPosition;
        var result = army.CanMoveTo(toPos);
        army.Position = startingPosition;
        return result;
    }

    public void UndoMove(int armyId)
    {
        var army = GetArmy(armyId);
        if (army != null)
        {
            army.Position = UnmovedPosition(army);
        }
    }

    public void CommitMoves()
    {
        originals.Clear();
    }

    public void MoveArmy(int armyId, Pos toPos)
    {
        var army = GetArmy(armyId);
        if (!originals.ContainsKey(army))
        {
            originals.Add(army, army.Position);
        }

        army.Position = toPos;
    }

    public void RemoveArmy(Army army)
    {
        if (ArmyList.Contains(army))
        {
            ArmyList.Remove(army);
        }
    }

    public void RenderArmies()
    {
        foreach (var army in ArmyList)
        {
            Color.Use();
            army.Render();
        }
    }

    public string ResourcesString()
    {
        return Resources.ToString();
    }

    public void Tick()
    {
        foreach (var army in ArmyList)
        {
            army.Tick();
        }
    }

    private Pos UnmovedPosition(Army army)
    {
        Pos? storedPosition;
        originals.TryGetValue(army, out storedPosition);
        return storedPosition.HasValue ? storedPosition.Value : army.Position;
    }
}