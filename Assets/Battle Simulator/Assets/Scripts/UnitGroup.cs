using System;
using System.Collections.Generic;

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

    public int[] GetUnitsTileIndex()
    {
        int[] indices = new int[units.Count];
        for(int i = 0; i < indices.Length; i++)
        {
            indices[i] = GridManager.Instance.GetTileWithPos(units[i].transform.position);
        }
        return indices;
    }

    public void RemoveAllUnit()
    {
        units.Clear();
    }
}
