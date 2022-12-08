using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
    Above,
    Below
}
public abstract class GridUnit : MonoBehaviour
{
    public virtual UnitType unitType { get { return UnitType.Empty; } }
    [ShowInInspector]
    public Dictionary<string, string> setting = new Dictionary<string, string>();
    public Cell cell;


    protected virtual bool MoveTo(Direction direction)
    {
        var targetCell = cell.grid.GetClosestCell(this.cell, direction);
        if(targetCell == null)
        {
            Debug.LogWarning(cell + " move " + direction + "failed, cannot get targetCell");
            return false;
        }
        if(Rules.Instance.CheckEnterCell(this.cell, targetCell)){
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
        this.transform.DOMove(targetCell.CellToWorld(targetCell), 1f);
    }
    protected Vector3 DirectionToWorld(Direction direction) {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.forward * (cell.grid.size + cell.grid.spacing);
            case Direction.Down:
                return Vector3.back *(cell.grid.size + cell.grid.spacing);
            case Direction.Left:
                return Vector3.left * (cell.grid.size + cell.grid.spacing);
            case Direction.Right:
                return Vector3.right * (cell.grid.size + cell.grid.spacing);
            case Direction.Above:
                return Vector3.up * (cell.grid.size + cell.grid.spacing);
            case Direction.Below:
                return Vector3.down * (cell.grid.size + cell.grid.spacing);
            default:
                return Vector3.zero;
        }
    }
}
