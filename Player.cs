using System.Collections.Generic;

public class Player
{
    public Player()
    {
        this.ArmyList = new List<Army>();
    }

    private List<Army> ArmyList { get; set; }

    public void AddArmy(Army army, Pos pos)
    {
        // create new army object
        this.ArmyList.Add(new Army(pos, 100));
    }

    public void MoveArmy(Army army, Pos toPos)
    {
        // TODO: Change
        if (this.ArmyList.Contains(army))
        {
            army.Position = toPos;
        }
    }

    public void RemoveArmy(Army army)
    {
        if (this.ArmyList.Contains(army))
        {
            this.ArmyList.Remove(army);
        }
    }
}