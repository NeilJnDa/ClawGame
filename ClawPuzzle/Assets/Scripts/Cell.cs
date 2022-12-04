using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public enum UnitType
{
    Empty = 0,
    Ground = 1,
    Player = 2,
    Hole = 3,
    Claw = 4,
}

public class Cell
{
    //pos.x: [0, length-1]
    //pos.y: [0, width-1]
    //pos.z: [0, height-1]
    public Vector3 pos { get; private set; }
    public Grid3D grid { get; private set; }
    public UnitType initUnitType { get; private set; }
    public GridUnit currentGridUnit { get; private set; }

    public Cell(int i, int j, int k, Grid3D grid, UnitType unitType)
    {
        this.grid = grid;
        this.initUnitType = unitType;
        pos = new Vector3(i, j, k);
    }
    public void Initialize()
    {
        //Length: axis_x
        //Width: axis_z
        //height: axis_y

        if (initUnitType == UnitType.Empty) return;
        currentGridUnit = GameObject.Instantiate(Resources.Load(initUnitType.ToString(), typeof(GridUnit))) as GridUnit;
        currentGridUnit.cell = this;
        currentGridUnit.transform.position =
            grid.parentTransform.position + grid.offset + 
            new Vector3(pos.x * (grid.size + grid.spacing), pos.z * (grid.size + grid.spacing), pos.y * (grid.size + grid.spacing));
        currentGridUnit.transform.parent = grid.parentTransform;
    }
}
