using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public enum UnitType
{
    Empty = 0,
    Ground = 1,
    Player = 2
}

public class Cell
{
    [ReadOnly]
    //pos.x: [0, length-1]
    //pos.y: [0, width-1]
    //pos.z: [0, height-1]
    public Vector3 pos;
    [ReadOnly]
    public Grid3D grid;
    [ReadOnly]
    public UnitType unitType;
    [ReadOnly]
    public GameObject unitObject;

    public Cell(int i, int j, int k, Grid3D grid, UnitType unitType)
    {
        this.grid = grid;
        this.unitType = unitType;
        pos.x = i;
        pos.y = j;
        pos.z = k;
    }
    public void Initialize()
    {
        //Length: axis_x
        //Width: axis_z
        //height: axis_y

        if (unitType == UnitType.Empty) return;
        unitObject = GameObject.Instantiate(Resources.Load(unitType.ToString(), typeof(GameObject))) as GameObject;
        unitObject.transform.position =
            grid.parentTransform.position + grid.offset + 
            new Vector3(pos.x * (grid.size + grid.spacing), pos.z * (grid.size + grid.spacing), pos.y * (grid.size + grid.spacing));
        unitObject.transform.parent = grid.parentTransform;
    }
}
