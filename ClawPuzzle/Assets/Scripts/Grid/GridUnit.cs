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


    public Pair[] setting = new Pair[5];

    public Cell cell;

    public void Initialize(CellInfo unitInfo)
    {
        //Creation and placing is done by its cell
        //Here we deal with the solid surface
        unitInfo.setting?.CopyTo(this.setting, 0);
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
        return Rules.Instance.CheckEnterCell(this.cell, targetCell, direction);
    }
    protected virtual void MoveToCell(Cell targetCell, float duration)
    {
        this.cell.Leave();
        targetCell.Occupy(this);
        Debug.Log(this.name + " move to " + targetCell.name + " succeeded");
        this.transform.DOMove(targetCell.CellToWorld(targetCell), duration);
    }

    #region ITurnUnit
    [ShowInInspector][ReadOnly]
    Stack<Cell> cellHistory = new Stack<Cell>();
    [ShowInInspector][ReadOnly]
    Stack<Pair[]> settingHistory = new Stack<Pair[]>();
    public void UndoOneStep()
    {
        cell = cellHistory.Pop();
        settingHistory.Pop().CopyTo(setting, 0);
    }

    public void ResetAll()
    {
        while (cellHistory.Count > 1)
        {
            cellHistory.Pop();
            settingHistory.Pop();
        }
        cell = cellHistory.Pop();
        settingHistory.Pop().CopyTo(setting, 0);

    }

    public void NextStep()
    {
        Cell cellRef = cell;
        cellHistory.Push(cellRef);

        Pair[] settingTemp = new Pair[5];
        setting.CopyTo(settingTemp, 0);
        settingHistory.Push(settingTemp);
    }
    #endregion
}
