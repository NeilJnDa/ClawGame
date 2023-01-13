using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum ClawState
{
    ReadyDrop,
    ReadyRaise,
    Raising
}
public class Claw : GridUnit
{
    public ClawState clawState = ClawState.ReadyDrop;
    public override UnitType unitType { get { return UnitType.Claw; } }
    public override bool catchable { get { return false; } }

    [field:SerializeField]
    public GridUnit HoldingUnit { get; private set; } = null;

    //Local cache of targetCell when moving
    private Cell targetCell;

    [Tooltip("When empty How many times speed of normal")]
    public float emptySpeedMultiplier = 4f;
    [Tooltip("When with an object, How many times speed of normal")]
    public float holdingSpeedMultiplier = 1f;

    private void Start()
    {
        InputManager.Instance.moveEvent += OnMove;
        InputManager.Instance.clawActionEvent += OnClawAction;
        TurnManager.Instance.PlayerTurnEvent += OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent += OnEnvTurn;
    }
    private void OnDestroy()
    {
        InputManager.Instance.moveEvent -= OnMove;
        InputManager.Instance.clawActionEvent -= OnClawAction;
        TurnManager.Instance.PlayerTurnEvent -= OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent -= OnEnvTurn;
    }

    private void OnPlayerTurn()
    {
        float distance = cell.grid.AbsoluteDistance(this.cell, targetCell);
        float duration = TurnManager.Instance.playerTurnDuration;
        if (clawState == ClawState.ReadyDrop || clawState == ClawState.ReadyRaise)
            duration *= distance / emptySpeedMultiplier;
        if(clawState == ClawState.Raising)
            duration *= distance / holdingSpeedMultiplier;
        duration = Mathf.Clamp(duration, 0, TurnManager.Instance.playerTurnDuration);
        MoveToCell(targetCell, duration);
    }
    private void OnEnvTurn()
    {
        if(clawState == ClawState.Raising)
        {
            if (CheckMoveToNext(Direction.Above))
            {
                MoveToCell(cell.grid.GetClosestCell(this.cell, Direction.Above), TurnManager.Instance.envTurnDuration);
            }
        }
    }

    private void OnMove(Direction direction)
    {
        if (this.CheckMoveToNext(direction))
        {
            targetCell = cell.grid.GetClosestCell(this.cell, direction);
            TurnManager.Instance.NextStep();
        }
    }
    private void OnClawAction()
    {
        if(clawState == ClawState.ReadyDrop)
        {
            if (CheckMoveClawToEnd(Direction.Below) != null)
            {
                TurnManager.Instance.NextStep();
            }
            else
            {
                //TODO: can not move animation
            }
        }
        else if(clawState == ClawState.ReadyRaise)
        {

        }
        else if(clawState == ClawState.Raising)
        {

        }
    }
    /// <summary>
    /// Move claw as far as possible in the desired direction. Return targetCell if it can move at least one unit
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Cell CheckMoveClawToEnd(Direction direction)
    {
        targetCell = null;
        Cell currentCell = this.cell;

        //Check cells of this direction one by one until find the last plausible one
        while (true)
        {
            targetCell = cell.grid.GetClosestCell(currentCell, direction);
            if (targetCell == null)
            {
                break;
            }
            else if(Rules.Instance.CheckClawEnterCell(currentCell, targetCell, direction))
            {
                currentCell = targetCell;
            }
            else
            {
                targetCell = currentCell;
                break;
            }
        }
        return targetCell;
    }
    


}
