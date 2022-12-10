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
public abstract class GridUnit : MonoBehaviour
{

    public virtual UnitType unitType { get { return UnitType.Empty; } }

    public Pair[] setting = new Pair[5];

    public Cell cell;

    public void Initialize(CellInfo unitInfo)
    {
        //Creation and placing is done by its cell
        //Here we deal with the solid surface
        unitInfo.setting?.CopyTo(this.setting, 0);

    }
    protected virtual bool MoveTo(Direction direction)
    {
        var targetCell = cell.grid.GetClosestCell(this.cell, direction);
        if (targetCell == null)
        {
            Debug.LogWarning(cell + " move " + direction + "failed, cannot get targetCell");
            return false;
        }
        if (Rules.Instance.CheckEnterCell(this.cell, targetCell, direction))
        {
            cell.Leave();
            targetCell.Occupy(this);
            MoveToAnim(targetCell);
            Debug.Log(cell + " move " + direction + " succeeded");
            return true;
        }
        else
        {
            Debug.LogWarning(cell + " move " + direction + " failed, Rules not allowed since target is " + targetCell.currentGridUnit);
            return false;
        }
    }
    protected virtual void MoveToAnim(Cell targetCell)
    {
        this.transform.DOMove(targetCell.CellToWorld(targetCell),
            unitType == UnitType.Claw ? TurnManager.Instance.playerTurnDuration : TurnManager.Instance.envTurnDuration);
    }
}
    