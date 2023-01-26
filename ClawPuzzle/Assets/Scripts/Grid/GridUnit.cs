using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;
using System;

[System.Serializable]
public enum UnitType
{
    Empty = 0,
    Ground = 1,
    Player = 2,
    Hole = 3,
    Claw = 4,
    Conveyor = 5,
    LimiterGround = 6,
    LimiterBox = 7,
    Box = 8,
    LongBox = 9,
    HoleBottom = 10,
}

/// <summary>
/// The actual object in the cell
/// </summary>
public abstract class GridUnit : MonoBehaviour
{
    //Must be overrided
    public virtual UnitType unitType { get { return UnitType.Empty; } }
    public virtual bool pushable { get { return false; } }

    //Initial setting cache, saved for reset
    [ShowInInspector][ReadOnly]
    protected GridUnitInfo initGridUnitInfo;
    [ShowInInspector]
    [ReadOnly]
    protected Cell initCell;

    //Current Grid Setting
    public List<Pair> setting;
    //Current Parent Cell
    public Cell cell;


    public void Initialize(GridUnitInfo gridUnitInfo, Grid3D grid)
    {
        //Creation and placing is done by its cell
        //Here we deal with the solid surface

        initGridUnitInfo = new GridUnitInfo(gridUnitInfo);

        cell = grid.cellMatrix[gridUnitInfo.i, gridUnitInfo.j, gridUnitInfo.k];
        initCell = cell;

        if (gridUnitInfo.setting != null)
            setting = new List<Pair>(gridUnitInfo.setting);
        else setting = new List<Pair>();

        transform.position = cell.transform.position;
        transform.parent = grid.parentTransform;
        cell.Enter(this);
    }

    #region Move and Check
    /// <summary>
    /// Iteration of checking if a unit can move into a cell and push
    /// </summary>
    /// <param name="gridUnit"></param>
    /// <param name="from"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual bool CheckMoveAndPushToNext(GridUnit gridUnit, Cell from, Direction direction)
    {
        var targetCell = from.NextCell(direction);
        if (targetCell == null)
        {
            Debug.Log("Checking: " + gridUnit.name + " Move and Push " + direction + "failed, can not get target cell" );
            return false;
        }
        Debug.Log("Checking: " + gridUnit.name + " Move and Push into" + targetCell.name);

        bool success = true;
        foreach (var unit in targetCell.gridUnits)
        {       
            if (!Rules.Instance.CheckCompatible(gridUnit, unit, from, targetCell, direction, true)) {
                Debug.Log(gridUnit.name + " can not enter " + targetCell.name);
                return false; 
            }
            else if(unit.pushable)
            {
                //This is an iteration to check all pushed units can move.
                success = success && unit.CheckMoveAndPushToNext(unit, targetCell, direction);
                if (success == false) return false;
            }
        }
        return success;
    }
    public virtual bool CheckMoveNoPush(GridUnit gridUnit, Cell from, Direction direction)
    {
        var targetCell = from.NextCell(direction);
        if (targetCell == null)
        {
            Debug.Log("Checking: " + gridUnit.name + " Move and No Push " + direction + "failed, can not get target cell");
            return false;
        }
        Debug.Log("Checking: " + gridUnit.name + " Move but No Push into" + targetCell.name);

        foreach (var unit in targetCell.gridUnits)
        {
            if (!Rules.Instance.CheckCompatible(gridUnit, unit, from, targetCell, direction, true))
            {
                //No Iteration, only check once
                
                Debug.Log(gridUnit.name + " can not enter " + targetCell.name);
                return false;
            }
        }
        return true;
    }

   

    /// <summary>
    /// Can be overrided for different animation
    /// </summary>
    /// <param name="targetCell"></param>
    /// <param name="duration"></param>
    public virtual void MoveToCell(Cell targetCell, Direction direction, float duration)
    {
        CheckMoveUnitsAbove(direction, duration);
        this.cell.Leave(this);
        targetCell.Enter(this);
        Debug.Log(this.name + " move to " + targetCell.name + " succeeded");
        this.transform.DOMove(targetCell.CellToWorld(targetCell), duration).SetEase(Ease.Linear);
    }

    private void CheckMoveUnitsAbove(Direction direction, float duration)
    {
        Cell upperCell = this.cell.NextCell(Direction.Above);
        if (upperCell == null) return;
        foreach(var unit in upperCell.gridUnits.ToArray())
        {
            if(unit.pushable && !((PushableGridUnit)unit).isCaught)
            {
                if(unit.CheckMoveNoPush(unit, upperCell, direction))
                {
                    Debug.Log("Carry move : " + unit.name + " is tweenning? " + DOTween.IsTweening(unit.transform));
                    if(!DOTween.IsTweening(unit.transform))
                        unit.MoveToCell(upperCell.NextCell(direction), direction, duration);
                }
            }
        }
    }
    #endregion

}
