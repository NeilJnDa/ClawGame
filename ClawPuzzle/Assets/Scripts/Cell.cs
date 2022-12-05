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
[System.Serializable]
public class Cell
{
    //pos.x: [0, length-1]
    //pos.y: [0, width-1]
    //pos.z: [0, height-1]
    [field: SerializeField, ReadOnly]
    public Vector3Int pos { get; private set; }
    [HideInInspector]
    public Grid3D grid { get; private set; }
    [HideInInspector]
    public UnitType initUnitType { get; private set; }
    [field: SerializeField, ReadOnly]
    public GridUnit currentGridUnit { get; private set; }

    public Cell(int i, int j, int k, Grid3D grid, UnitType unitType)
    {
        this.grid = grid;
        this.initUnitType = unitType;
        pos = new Vector3Int(i, j, k);
    }
    public void Initialize()
    {
        //Length pos_x : axis_x
        //Width pos_y : axis_z
        //height pos_z : axis_y

        if (initUnitType == UnitType.Empty) return;
        currentGridUnit = GameObject.Instantiate(Resources.Load(initUnitType.ToString(), typeof(GridUnit))) as GridUnit;
        currentGridUnit.cell = this;
        currentGridUnit.transform.position =
            grid.parentTransform.position + grid.offset + 
            new Vector3(pos.x * (grid.size + grid.spacing), pos.z * (grid.size + grid.spacing), pos.y * (grid.size + grid.spacing));
        currentGridUnit.transform.parent = grid.parentTransform;
    }

    public void Occupy(GridUnit newUnit)
    {
        currentGridUnit = newUnit;
        newUnit.cell = this;
    }
    public void Leave()
    {
        currentGridUnit = null;
    }
    public Vector3 CellToWorld(Cell cell)
    {
        //Length pos_x : axis_x
        //Width pos_y : axis_z
        //height pos_z : axis_y
        return cell.grid.parentTransform.position + cell.grid.offset +
            new Vector3(cell.pos.x, cell.pos.z, cell.pos.y) * (cell.grid.spacing + cell.grid.size);
    }
    
}
