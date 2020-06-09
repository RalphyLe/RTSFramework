using Boo.Lang;
using System;
public class UnitGroup
{
    public List<Unit> units;
    public string tag;
    public UnitGroup(string tag)
    {
        units = new List<Unit>();
        this.tag = tag;
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        units.Remove(unit);
    }
}
