using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManager:MonoBehaviour
{
    public GameObject tileObj;
    public Vector3 placeCenter;
    public int curIndex = -1;
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
        this.gridCenter = new Vector2(placeCenter.x, placeCenter.z);
        this.gridSize = gridSize;

        for (int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                Vector2 pos = new Vector2(i * tileSize, j * tileSize - gridSize);
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

    public void SetCurrentTileUnit(Unit unit)
    {
        if (curIndex >= 0)
            grids[curIndex].unit = unit;
    }

    private void OnDrawGizmos()
    {
        if (grids.Count > 0)
        {
            for(int i = 0; i < grids.Count; i++)
            {
                Gizmos.DrawWireCube(new Vector3(grids[i].rect.center.x, 0, grids[i].rect.center.y), Vector3.one * tileSize);
            }
        }
    }

    public Vector2 GetTilePos(int id)
    {
        return grids[id].rect.center;
    }

    public bool HasPlaceUnit(int id)
    {
        if (id < 0)
            return true;
        return grids[id].unit != null;
    }

    private void Update()
    {
        
    }
}
