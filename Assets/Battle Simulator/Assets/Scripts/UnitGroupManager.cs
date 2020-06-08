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
    }

    public void CreateNewGroup(string tag)
    {
        if (groups == null)
            groups = new List<UnitGroup>();
        groups.Add(new UnitGroup(tag));
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
