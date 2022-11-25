using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public enum CellType
{
    Default = 0,
    Empty = 1
}

public class Cell
{
    [ReadOnly]
    //pos.x: [0, length-1]
    //pos.y: [0, width-1]
    public Vector2 pos;
    [ReadOnly]
    public Grid2D grid;
    [ReadOnly]
    public CellType cellType;
    [ReadOnly]
    public GameObject cellObject;

    public Cell(int i, int j, Grid2D grid, CellType cellType)
    {
        this.grid = grid;
        this.cellType = cellType;
        pos.x = i;
        pos.y = j;
    }
    public void Initialize()
    {
        cellObject = GameObject.Instantiate(Resources.Load(cellType.ToString(), typeof(GameObject))) as GameObject;
        cellObject.transform.position =
            grid.parentTransform.position +
            new Vector3(pos.x * (grid.size + grid.spacing), 0, pos.y * (grid.size + grid.spacing));
        cellObject.transform.parent = grid.parentTransform;
    }
}
