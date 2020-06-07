using UnityEngine;
using System.Collections;

public class Tile
{
    public Unit unit = null;
    public Rect rect;
    public GameObject instance;
    public int index;

    public Tile(int id,Rect rect)
    {
        this.index = id;
        this.rect = rect;
    }

    public bool isInRect(Vector2 point)
    {
        return rect.Contains(point);
    }

    public bool inInRect(Vector3 point)
    {
        return rect.Contains(point);
    }
    
}
