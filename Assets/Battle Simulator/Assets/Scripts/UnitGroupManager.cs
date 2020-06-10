using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitGroupManager : MonoBehaviour
{
    public static UnitGroupManager Instance;
    public List<UnitGroup> groups;
    // Use this for initialization
    void Start()
    {
        Instance = this;
        groups = new List<UnitGroup>();
    }

    public UnitGroup CreateNewGroup(string tag)
    {
        if (groups == null)
            groups = new List<UnitGroup>();
        var group = new UnitGroup(tag);
        group.index = groups.Count;
        groups.Add(group);
        return group;
    }

    public UnitGroup GetValidGroup(string tag)
    {
        for(int i = 0; i < groups.Count; i++)
        {
            if (groups[i].tag == tag && groups[i].units.Count == 0)
                return groups[i];
        }
        return Instance.CreateNewGroup(tag);
    }

    public void ClearGroups()
    {
        groups.Clear();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
