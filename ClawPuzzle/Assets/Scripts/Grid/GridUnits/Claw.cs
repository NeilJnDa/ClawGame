using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public enum ClawState
{
    ReadyDrop,
    ReadyRaise,
    RaisingStart,
    Raising,
    Released
}
public class Claw : GridUnit
{
    public ClawState clawState = ClawState.ReadyDrop;
    public override UnitType unitType { get { return UnitType.Claw; } }
    public override bool catchable { get { return false; } }

    [field:SerializeField]
    public List<GridUnit> HoldingUnits { get; private set; } = null;

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
        TurnManager.Instance.CheckInteractionEvent += OnCheckInteraction;
        TurnManager.Instance.EndStepProcessEvent += OnEndStep;
    }
    private void OnDestroy()
    {
        InputManager.Instance.moveEvent -= OnMove;
        InputManager.Instance.clawActionEvent -= OnClawAction;

        TurnManager.Instance.PlayerTurnEvent -= OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent -= OnEnvTurn;
        TurnManager.Instance.CheckInteractionEvent -= OnCheckInteraction;
        TurnManager.Instance.EndStepProcessEvent -= OnEndStep;


    }

    private void OnPlayerTurn()
    {
        if(targetCellCache == null || targetCellCache == this.cell)
        {
            //No need to move
            return;
        }
        //Calculate move duration

        float distance = cell.grid.AbsoluteDistance(this.cell, targetCellCache);
        float duration = TurnManager.Instance.playerTurnDuration;
        if (clawState == ClawState.ReadyDrop || clawState == ClawState.ReadyRaise)
            duration *= distance / emptySpeedMultiplier;
        if(clawState == ClawState.Raising)
            duration *= distance / holdingSpeedMultiplier;
        duration = Mathf.Clamp(duration, 0, TurnManager.Instance.playerTurnDuration);

        //Move Claw
        MoveToCell(targetCellCache, duration);
        //Move all Holding Units as well, No Rule Checking
        foreach(var unit in HoldingUnits)
        {
            unit.MoveToCell(targetCellCache, duration);
        }
        //Clear Cache
        targetCellCache = null;
    }
    private void OnEnvTurn()
    {
        if(clawState == ClawState.Raising)
        {
            if (CheckMoveToNext(Direction.Above))
            {
                MoveToCell(targetCellCache, TurnManager.Instance.envTurnDuration);
                foreach (var unit in HoldingUnits)
                {
                    unit.MoveToCell(targetCellCache, TurnManager.Instance.envTurnDuration);
                }
            }
        }
        //Clear Cache
        targetCellCache = null;
    }
    private void OnCheckInteraction()
    {
        //Claw checks if there are something to catch
        //Both when ReadyDrop and ReadyRaise, it can claw something

        if (clawState == ClawState.ReadyDrop || clawState == ClawState.ReadyRaise)
        {
            //It has already entered the cell (after the player turn) where there is something to catch
            
            var toCatchList = cell.gridUnits.FindAll(x => x.catchable == true);
            foreach(var toCatch in toCatchList)
            {
                if (HoldingUnits.Contains(toCatch)) continue;
                HoldingUnits.Add(toCatch);
                Debug.Log(this.name + " catch " + toCatch.name);
            }
            if (HoldingUnits.Count > 0) clawState = ClawState.RaisingStart;
        }
    }
    private void OnEndStep()
    {
        if (clawState == ClawState.RaisingStart && HoldingUnits.Count > 0)
            clawState = ClawState.Raising;
        if (clawState == ClawState.Released) clawState = ClawState.ReadyDrop;
    }
    private void OnMove(Direction direction)
    {
        if (this.CheckMoveToNext(direction))
        {
            TurnManager.Instance.NextStep();
        }
    }
    private void OnClawAction()
    {
        if(clawState == ClawState.ReadyDrop)
        {
            if (CheckMoveToEnd(Direction.Below))
            {
                clawState = ClawState.ReadyRaise;
                TurnManager.Instance.NextStep();
            }
            else
            {
                //TODO: can not move animation
            }
        }
        else if(clawState == ClawState.ReadyRaise)
        {
            if (CheckMoveToEnd(Direction.Above))
            {
                clawState = ClawState.ReadyDrop;
                TurnManager.Instance.NextStep();
            }
            else
            {
                //TODO: can not move animation
            }
        }
        else if(clawState == ClawState.Raising)
        {
            //Release claw
            clawState = ClawState.Released;
            foreach(var unit in HoldingUnits)
            {
                if (unit.CheckMoveToEnd(Direction.Below))
                {
                    //Cache will be set. The unit will move when player turn
                }
            }
            HoldingUnits.Clear();
            TurnManager.Instance.NextStep();
        }
    }
    
    
    
}
