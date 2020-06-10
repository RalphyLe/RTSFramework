using Boo.Lang;
using System;
public class UnitGroup
{
    public List<Unit> units;
    public string tag;
    public int index;
    public UnitGroup(string tag)
    {
        units = new List<Unit>();
        this.tag = tag;
    }

    public void AddUnit(Unit unit)
    {
        unit.groupId = index;
        units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        if(units.Contains(unit))
            units.Remove(unit);
    }

    public void RemoveAllUnit()
    {
        units.Clear();
    }
}
