using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;

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
    LongBox = 9
}

/// <summary>
/// The actual object in the cell
/// </summary>
public abstract class GridUnit : MonoBehaviour
{
    //Must be overrided
    public virtual UnitType unitType { get { return UnitType.Empty; } }
    public virtual bool catchable { get { return false; } }
    public virtual bool pushable { get { return false; } }

    //Initial setting cache
    [ShowInInspector][ReadOnly]
    protected GridUnitInfo initGridUnitInfo;
    [ShowInInspector]
    [ReadOnly]
    protected Cell initCell;

    //Current Grid Setting
    public List<Pair> setting;
    //Current Parent Cell
    public Cell cell;

    //A simple command cache. It will be set when player do something, and executed and cleared during player turn or env turn.
    public Cell targetCellCache = null;

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
        transform.parent = cell.transform;
        cell.Enter(this);
    }

    #region Move and Check
    /// <summary>
    /// Check if can move to an adjacent cell
    /// If true, the targetCellCache will be set
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CheckMoveToNext(Direction direction, bool ignorePushable = false)
    {
        var targetCell = cell.grid.GetClosestCell(this.cell, direction);
        if (targetCell == null)
        {
            Debug.LogWarning(cell + " move " + direction + "failed, cannot get targetCell");
            return false;
        }
        if (Rules.Instance.CheckEnterCell(this, this.cell, targetCell, direction, ignorePushable))
        {
            targetCellCache = targetCell;
            return true;
        };
        return false;
    }
    /// <summary>
    /// Check if can push units in an adjacent cell, and move. No chaining push.
    /// The height is especially for long units or the claw. Such as a 1x1x2 has height of 2
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public bool CheckMoveAndPushToNext(Cell from, Direction direction)
    {
        var targetCell = from?.grid.GetClosestCell(from, direction);
        var secondCell = targetCell?.grid.GetClosestCell(targetCell, direction);
        if (targetCell == null)
        {
            Debug.LogWarning(cell + " move " + direction + "failed, cannot get targetCell");
            return false;
        }
        //If second is null, only check move into next (no push)
        else if (secondCell == null)
        {
            if (Rules.Instance.CheckEnterCell(this, from, targetCell, direction))
            {
                targetCellCache = targetCell;
                return true;
            };
        }
        //Next two cells are not null, try to push
        else
        {
            //Check push
            if (
                //Check if all pushed units can enter the second cell
                targetCell.gridUnits.All(x => x.pushable && Rules.Instance.CheckEnterCell(x, targetCell, secondCell, direction))
                &&
                //Check if this grid unit can enter the next one when pushable units have left. Set IgnorePushable to true
                Rules.Instance.CheckEnterCell(this, from, targetCell, direction, true)
                )
            {
                //Push and set move cache
                targetCellCache = targetCell;
                targetCell.gridUnits.ForEach(x =>
                    {
                        if (x.pushable) x.targetCellCache = secondCell;
                    }
                );
                return true;
            }
            //All pushed units can not enter the second cell, No push, only check, enter
            else
            {

                if (Rules.Instance.CheckEnterCell(this, from, targetCell, direction))
                {
                    targetCellCache = targetCell;
                    return true;
                };
            }
        }

        return false;
    }
    public bool CheckMoveAndPushWithHeight(Cell from, Direction direction, int height = 1)
    {
        var cellsAbove = this.cell.grid.GetCellsFrom(this.cell, Direction.Above);
        for(int i = 0; i < height; ++i)
        {
            if (i >= cellsAbove.Count) continue;
            if(!CheckMoveAndPushToNext(cellsAbove[i], direction))
            {
                return false;
            }
        }
        //Every check is true, it can move and push with this height
        return true;
    }
    
    /// <summary>
    /// Move this unit as far as possible in the desired direction. Return true and set targetCell if it can move at least one unit
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public bool CheckMoveToEnd(Direction direction, bool isClawToCatch = false)
    {
        Cell targetCell = null;
        Cell currentCell = this.cell;

        //Check cells of this direction one by one until find the last plausible one
        while (true)
        {
            targetCell = cell.grid.GetClosestCell(currentCell, direction);
            if (targetCell != null && Rules.Instance.CheckEnterCell(this, currentCell, targetCell, direction, false, isClawToCatch))
            {
                //If this is a claw to catch, first time the next cell has a catachble object, end the iteraion
                if (isClawToCatch && targetCell.gridUnits.Exists(x => x.catchable)) break;

                //Continue the iteration
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
    /// <summary>
    /// Can be overrided for different animation
    /// </summary>
    /// <param name="targetCell"></param>
    /// <param name="duration"></param>
    public virtual void MoveToCell(Cell targetCell, float duration)
    {
        this.cell.Leave(this);
        targetCell.Enter(this);
        Debug.Log(this.name + " move to " + targetCell.name + " succeeded");
        this.transform.DOMove(targetCell.CellToWorld(targetCell), duration);
    }
    #endregion

    #region Limiter
    public virtual void OnLimitation()
    {
    }
    public virtual void OnEndLimitation()
    {
    }
    #endregion
}
