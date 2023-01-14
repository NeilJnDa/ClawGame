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
    Conveyor = 5
}

/// <summary>
/// The actual object in the cell
/// </summary>
public abstract class GridUnit : MonoBehaviour, ITurnUnit
{

    //Must be overrided
    public virtual UnitType unitType { get { return UnitType.Empty; } }
    public virtual bool catchable { get { return false; } }


    public List<Pair> setting;

    public Cell cell;

    public void Initialize(GridUnitInfo gridUnitInfo)
    {
        //Creation and placing is done by its cell
        //Here we deal with the solid surface
        if (gridUnitInfo.setting != null) setting = gridUnitInfo.setting;
        else setting = new List<Pair>();
    }
    /// <summary>
    /// Check if can move to an adjacent cell
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    protected virtual bool CheckMoveToNext(Direction direction)
    {
        var targetCell = cell.grid.GetClosestCell(this.cell, direction);
        if (targetCell == null)
        {  
            Debug.LogWarning(cell + " move " + direction + "failed, cannot get targetCell");
            return false;
        }
        return Rules.Instance.CheckEnterCell(this, this.cell, targetCell, direction);
    }
    protected virtual void MoveToCell(Cell targetCell, float duration)
    {
        this.cell.Leave(this);
        targetCell.Enter(this);
        Debug.Log(this.name + " move to " + targetCell.name + " succeeded");
        this.transform.DOMove(targetCell.CellToWorld(targetCell), duration);
    }

    #region ITurnUnit
    [ShowInInspector][ReadOnly]
    Stack<Cell> cellHistory = new Stack<Cell>();
    [ShowInInspector]
    [ReadOnly]
    Stack<List<Pair>> settingHistory = new Stack<List<Pair>>();
    public void UndoOneStep()
    {
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
