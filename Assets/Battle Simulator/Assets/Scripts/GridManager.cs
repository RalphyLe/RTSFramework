using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager:MonoBehaviour
{
    [HideInInspector]
    public List<Tile> grids = new List<Tile>();
    [HideInInspector]
    public static GridManager Instance;
    private float tileSize = 2f;
    private Vector2 gridCenter;
    private int gridSize = 10;
    
    private void Start()
    {
        Instance = this;
    }

    public void InitilizeGrid(Vector3 start,int gridSize)
    {
        if (grids == null)
            grids = new List<Tile>();
        grids.Clear();
        this.gridCenter = new Vector2(start.x, start.z);
        this.gridSize = gridSize;

        for (int i = -gridSize; i < gridSize; i++)
        {
            for(int j = -gridSize; j < gridSize; j++)
            {
                Vector2 pos = new Vector2(this.gridCenter.x - i * tileSize, this.gridCenter.y - j * tileSize);
                grids.Add(new Tile(gridSize * i + j, new Rect(pos, Vector2.one * tileSize)));
            }
        }
    }

    public int GetTileWithPos(Vector2 point)
    {
        for(int i = 0; i < grids.Count; i++)
        {
            var tile = grids[i];
            if(tile.isInRect(point))
                return i;
        }
        return -1;
    }

    public int GetTileWithPos(Vector3 point)
    {
        Vector2 trans = new Vector2(point.x, point.z);
        return GetTileWithPos(trans);
    }

    private void OnDrawGizmos()
    {

    }

    public Vector2 GetTilePos(int id)
    {
        return grids[id].rect.center;
    }

    public bool HasPlaceUnit(int id)
    {
        return grids[id].unit != null;
    }

    private void Update()
    {
        
    }


}
