using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

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
    LimiterBox = 7
}

/// <summary>
/// The actual object in the cell
/// </summary>
public abstract class GridUnit : MonoBehaviour, ITurnUndo
{
    //Must be overrided
    public virtual UnitType unitType { get { return UnitType.Empty; } }
    public virtual bool catchable { get { return false; } }
    public virtual bool pushable { get { return false; } }


    //Grid Setting
    public List<Pair> setting;
    //Current Parent Cell
    public Cell cell;

    //A simple command cache. It will be set when player do something, and executed and cleared during player turn or env turn.
    public Cell targetCellCache = null;

    public void Initialize(GridUnitInfo gridUnitInfo)
    {
        //Creation and placing is done by its cell
        //Here we deal with the solid surface
        if (gridUnitInfo.setting != null) setting = gridUnitInfo.setting;
        else setting = new List<Pair>();
    }

    #region Move and Check
    /// <summary>
    /// Check if can move to an adjacent cell
    /// If true, the targetCellCache will be set
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CheckMoveToNext(Direction direction)
    {
        var targetCell = cell.grid.GetClosestCell(this.cell, direction);
        if (targetCell == null)
        {  
            Debug.LogWarning(cell + " move " + direction + "failed, cannot get targetCell");
            return false;
        }
        if (Rules.Instance.CheckEnterCell(this, this.cell, targetCell, direction))
        {
            targetCellCache = targetCell;
            return true;
        };
        return false;
    }
    public bool CheckMoveAndPushToNext(Direction direction)
    {
        var targetCell = cell.grid.GetClosestCell(this.cell, direction);
        if (targetCell == null)
        {
            Debug.LogWarning(cell + " move " + direction + "failed, cannot get targetCell");
            return false;
        }
        if (Rules.Instance.CheckEnterCell(this, this.cell, targetCell, direction))
        {
            targetCellCache = targetCell;
            return true;
        };
        return false;
    }
    /// <summary>
    /// Move this unit as far as possible in the desired direction. Return true and set targetCell if it can move at least one unit
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CheckMoveToEnd(Direction direction)
    {
        Cell targetCell = null;
        Cell currentCell = this.cell;

        //Check cells of this direction one by one until find the last plausible one
        while (true)
        {
            targetCell = cell.grid.GetClosestCell(currentCell, direction);
            if (targetCell != null && Rules.Instance.CheckEnterCell(this, currentCell, targetCell, direction))
            {
                currentCell = targetCell;
            }
            else
            {
                targetCell = currentCell;
                break;
            }
        }
        if (targetCell != null && targetCell != this.cell)
        {
            targetCellCache = targetCell;
            return true;
        }
        return false;
    }
    public virtual void MoveToCell(Cell targetCell, float duration)
    {
        this.cell.Leave(this);
        targetCell.Enter(this);
        Debug.Log(this.name + " move to " + targetCell.name + " succeeded");
        this.transform.DOMove(targetCell.CellToWorld(targetCell), duration);
    }
    #endregion

    #region ITurnUnit
    [ShowInInspector][ReadOnly]
    Stack<Cell> cellHistory = new Stack<Cell>();
    [ShowInInspector]
    [ReadOnly]
    Stack<List<Pair>> settingHistory = new Stack<List<Pair>>();
    public void UndoOneStep()
    {
        //TODO: Clear targetcell if necessary
        cell = cellHistory.Pop();
        setting = settingHistory.Pop();
    }

    public void ResetAll()
    {
        while (cellHistory.Count > 1)
        {
            cellHistory.Pop();
            settingHistory.Pop();
        }
        cell = cellHistory.Pop();
        setting = settingHistory.Pop();

    }

    public void NextStep()
    {
        Cell cellRef = cell;
        cellHistory.Push(cellRef);

        List<Pair> settingTemp = new List<Pair>(setting);
        settingHistory.Push(settingTemp);
    }
    #endregion
}
