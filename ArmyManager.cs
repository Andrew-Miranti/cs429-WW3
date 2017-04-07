using System.Collections.Generic;

public class ArmyManager
{
    private Dictionary<Army, Pos> positions;
    private Dictionary<Army, Pos> committedPositions;

    public ArmyManager()
    {
        positions = new Dictionary<Army, Pos>();
        committedPositions = new Dictionary<Army, Pos>();
    }

    public List<Army> GetArmies()
    {
        return new List<Army>(committedPositions.Keys);
    }

    public Pos ArmyPosition(Army army)
    {
        if (positions.ContainsKey(army))
        {
            return positions[army];
        }
        else
        {
            return committedPositions[army];
        }
    }

    public bool CanPlaceArmy(Army army, Pos pos)
    {
        foreach (var placed in GetArmies())
        {
            if (placed != army && ArmyPosition(placed).Equals(pos))
            {
                return false;
            }
        }

        return true;
    }

    public bool HasArmyMoved(Army army)
    {
        return positions.ContainsKey(army);
    }

    public void AddArmy(Army army, Pos pos)
    {
        if (CanPlaceArmy(army, pos))
        {
            committedPositions[army] = pos;
        }
    }

    public bool CanMoveTo(Army army, Pos dest)
    {
        return !army.Moved && ArmyPosition(army).IsInRange(dest, army.MoveRange);
    }

    public void MoveArmy(Army army, Pos dest)
    {
        if (CanPlaceArmy(army, dest))
        {
            var tentativePosition = ArmyPosition(army);
            positions[army] = committedPositions[army];
            if (CanMoveTo(army, dest))
            {
                positions[army] = dest;
            }
            else
            {
                positions[army] = tentativePosition;
            }
        }
    }

    public void Tick()
    {
        foreach (var pair in positions)
        {
            committedPositions[pair.Key] = pair.Value;
        }
    }

    public void UndoMove(Army army)
    {
        positions.Remove(army);
        if (positions.ContainsValue(committedPositions[army]))
        {
            UndoMove(ArmyAt(committedPositions[army]));
        }
    }

    public Army ArmyAt(Pos pos)
    {
        foreach (var army in GetArmies())
        {
            if (ArmyPosition(army).Equals(pos))
            {
                return army;
            }
        }

        return null;
    }

    public void RemoveArmy(Army army)
    {
        committedPositions.Remove(army);
        positions.Remove(army);
    }
}