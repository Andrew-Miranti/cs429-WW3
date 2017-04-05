using System.Collections.Generic;

public class Player
{
    private ArmyManager manager;

    public Player(ArmyManager man, Color c)
    {
        ArmyList = new List<Army>();
        Color = c;
        Resources = new ResourceBag();
        manager = man;
    }

    public List<Army> ArmyList { get; private set; }

    public Color Color { get; set; }

    public ResourceBag Resources { get; private set; }

    public void AddArmy(Army army, Pos dest)
    {
        // create new army object
        ArmyList.Add(army);
        manager.AddArmy(army, dest);
    }

    public bool CanPlaceArmy(Army army, Pos dest)
    {
        return manager.CanPlaceArmy(army, dest);
    }

    public Army GetArmy(int id)
    {
        if (ArmyExists(id))
        {
            return ArmyList[id];
        }

        return null;
    }

    public bool HasArmyActed(int id)
    {
        var army = GetArmy(id);
        if (army == null)
        {
            return false;
        }

        return manager.HasArmyMoved(army) || army.Moved;
    }

    public void ArmyActed(int id)
    {
        var army = GetArmy(id);
        if (army != null)
        {
            army.Moved = true;
        }
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
        return manager.ArmyPosition(GetArmy(armyId));
    }

    public bool CanMoveArmy(int armyId, Pos toPos)
    {
        var army = GetArmy(armyId);
        return army != null && !army.Moved && manager.CanMoveTo(army, toPos);
    }

    public void UndoMove(int armyId)
    {
        var army = GetArmy(armyId);
        if (army != null)
        {
            manager.UndoMove(army);
        }
    }

    public void MoveArmy(int armyId, Pos toPos)
    {
        var army = GetArmy(armyId);
        if (army != null)
        {
            manager.MoveArmy(army, toPos);
        }
    }

    public void RemoveArmy(Army army)
    {
        if (ArmyList.Contains(army))
        {
            ArmyList.Remove(army);
            manager.RemoveArmy(army);
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
}