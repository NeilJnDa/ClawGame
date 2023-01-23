using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public enum ClawState
{
    Open,
    Close
}
public class Claw : GridUnit, ITurnUndo
{
    [ReadOnly]
    public ClawState clawState = ClawState.Close;
    public override UnitType unitType { get { return UnitType.Claw; } }
    public override bool catchable { get { return false; } }
    public override bool pushable { get { return false; } }


    [field:SerializeField]
    public List<GridUnit> HoldingUnits { get; private set; } = null;
    /// <summary>
    /// Return true if the claw is holding something, false when holdingUnits are empty
    /// </summary>
    public bool isHolding => HoldingUnits.Count > 0;
    [field: SerializeField][ReadOnly]
    public bool isDropping { get; private set; } = false; //Temp flag when player input is Space, trying to catch something.

    [Tooltip("When empty How many times speed of normal")]
    public float emptySpeedMultiplier = 4f;
    [Tooltip("When with an object, How many times speed of normal")]
    public float holdingSpeedMultiplier = 1f;

    private void Start()
    {
        InputManager.Instance.moveEvent += OnMoveInput;
        InputManager.Instance.clawActionEvent += OnClawActionInput;
        InputManager.Instance.liftEvent += OnLiftInput;

        TurnManager.Instance.PlayerTurnEvent += OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent += OnEnvTurn;
        TurnManager.Instance.EndStepProcessEvent += OnEndStep;
        TurnManager.Instance.ClawOpenEvent += OnClawDroppingOpen;
        TurnManager.Instance.ClawCloseEvent += OnClawDroppingClose;
    }


    private void OnDestroy()
    {
        InputManager.Instance.moveEvent -= OnMoveInput;
        InputManager.Instance.clawActionEvent -= OnClawActionInput;
        InputManager.Instance.liftEvent -= OnLiftInput;

        TurnManager.Instance.PlayerTurnEvent -= OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent -= OnEnvTurn;
        TurnManager.Instance.EndStepProcessEvent -= OnEndStep;
        TurnManager.Instance.ClawOpenEvent -= OnClawDroppingOpen;
        TurnManager.Instance.ClawCloseEvent -= OnClawDroppingClose;

    }
    #region Input Event Subscribers
    private void OnMoveInput(Direction direction)
    {
        Cell upperCell = this.cell.grid.GetClosestCell(this.cell, Direction.Above);
        if (clawState == ClawState.Close)
        {
            //The claw Will Push
            if (this.CheckMoveAndPushToNext(this.cell, direction, true)&& 
                (upperCell == null || this.CheckPushWithHeightNoWriting(upperCell, direction, 9)))
            {
                TurnManager.Instance.NextStep();
            }
        }
        else if (clawState == ClawState.Open)
        {
            //Claw can enter, but the stick will push
            if (this.CheckMoveToNext(direction, true) &&
                 (upperCell == null || this.CheckPushWithHeightNoWriting(upperCell, direction, 9)))
            {
                TurnManager.Instance.NextStep();
            }
        }
    }
    private void OnLiftInput()
    {
        if (isHolding)
        {
            if (this.CheckMoveToNext(Direction.Above))
            {
                TurnManager.Instance.NextStep();
            }
        }
        else
        {
            if (this.CheckMoveToEnd(Direction.Above))
            {
                TurnManager.Instance.NextStep();
            }
        }

    }
    private void OnClawActionInput()
    {
        if (isHolding)
        {
            Release();
            TurnManager.Instance.NextStep();
        }
        else
        {
            //Check if there is something to catch in this cell. 
            if (CheckAndCatchUnit())
            {
                isDropping = true;
                TurnManager.Instance.NextStep();
            }
            //Otherwise, Check if it can move can catch something downwards
            //Even thought it can not move, it will act if the claw needs to close
            else if (CheckMoveToEnd(Direction.Below, true) || clawState == ClawState.Open)
            {
                isDropping = true;
                TurnManager.Instance.NextStep();
            }
            else
            {
                //TODO: can not move animation
            }
        }
    }

    #endregion
    #region Turn Event Subscribers
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
        if (isHolding)
            duration *= distance / holdingSpeedMultiplier;
        else
            duration *= distance / emptySpeedMultiplier;

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
        if (targetCellCache != null)
        {
            MoveToCell(targetCellCache, TurnManager.Instance.envTurnDuration);
            foreach (var unit in HoldingUnits)
            {
                unit.MoveToCell(targetCellCache, TurnManager.Instance.envTurnDuration);
            }
        }
        //Clear Cache
        targetCellCache = null;
    }

    private float OnClawDroppingClose()
    {
        if (isDropping)
        {
            var limiter = CheckLimitation();
            if (limiter != null)
            {
                //TODO: Failed to catch anim, limiter anim
                clawState = ClawState.Open;
                return 0.2f;
            }
            else
            {
                Catch();

                return 0.2f;
            }
        }
        return 0;
    }


    private float OnClawDroppingOpen()
    {
        if (isDropping && clawState == ClawState.Close)
        {
            Release();
            return 0.2f;
        }
        return 0;
    }
    /// <summary>
    /// Return true if new units are caught
    /// </summary>
    /// <returns></returns>

    private void OnEndStep()
    {
        if (isDropping) isDropping = false;
    }

    public override void OnLimitation()
    {
        Debug.Log(this.name + " on Limitation");
        base.OnLimitation();
        if (clawState == ClawState.Close)
        {
            Release();
        }
    }
    public override void OnEndLimitation()
    {
        base.OnEndLimitation();
        Debug.Log(this.name + " on End Limitation");
        if (clawState == ClawState.Open)
        {
            Catch();
        }
    }
    #endregion
    #region Behaviours
    public void Release()
    {
        //Release claw
        clawState = ClawState.Open;

        //TODO: Open Claw Animation
        foreach (var unit in HoldingUnits)
        {
            if (unit.CheckMoveToEnd(Direction.Below))
            {
                //Cache will be set. The unit will move when player turn
            }
        }
        HoldingUnits.Clear();
        Debug.Log(this.name + " Release Claw");
    }
    public void Catch()
    {
        if (CheckAndCatchUnit())
        {
            Debug.Log("Claw Caught units");
        }
        else
        {
            Debug.Log("Claw Caught nothing but still close");
        }
        clawState = ClawState.Close;
        //TODO: Close Claw Animation

    }
    private bool CheckAndCatchUnit()
    {
        var toCatchList = cell.gridUnits.FindAll(x => x.catchable == true);
        bool newCatch = false;
        foreach (var toCatch in toCatchList)
        {
            //Skip caught units 
            if (HoldingUnits.Contains(toCatch)) continue;
            HoldingUnits.Add(toCatch);
            Debug.Log(this.name + " catch " + toCatch.name);
            newCatch = true;
        }
        return newCatch;
    }

    private GridUnit CheckLimitation()
    {
        var cells = this.cell.grid.GetCellsFrom(this.cell, Direction.Below);
        foreach(var cell in cells)
        {
            foreach(var unit in cell.gridUnits)
            {
                if(unit.unitType == UnitType.LimiterBox || unit.unitType == UnitType.LimiterGround)
                {
                    return unit;
                }
            }
        }
        return null;
    }
    #endregion
    #region ITurnUndo
    [ShowInInspector]
    [ReadOnly]
    Stack<Cell> cellHistory = new Stack<Cell>();
    [ShowInInspector]
    [ReadOnly]
    Stack<List<Pair>> settingHistory = new Stack<List<Pair>>();
    [ShowInInspector]
    [ReadOnly]
    Stack<ClawState> clawStateHistory = new Stack<ClawState>();
    [ShowInInspector]
    [ReadOnly]
    Stack<List<GridUnit>> holdingUnitsHistory = new Stack<List<GridUnit>>();
    public void UndoOneStep()
    {
        cell.Leave(this);
        cell = cellHistory.Pop();
        cell.Enter(this);

        setting = settingHistory.Pop();
        clawState = clawStateHistory.Pop();
        HoldingUnits = holdingUnitsHistory.Pop();

        targetCellCache = null;
        this.transform.position = cell.transform.position;
    }

    public void ResetAll()
    {

        Cell cellRef = cell;
        cellHistory.Push(cellRef);
        List<Pair> settingTemp = new List<Pair>(setting);
        settingHistory.Push(settingTemp);
        clawStateHistory.Push(clawState);
        List<GridUnit> holdingUnitsTemp = new List<GridUnit>(HoldingUnits);
        holdingUnitsHistory.Push(holdingUnitsTemp);

        cell.Leave(this);
        cell = initCell;
        cell.Enter(this);
        if (initGridUnitInfo.setting == null)
        {
            setting.Clear();
        }
        else
        {
            setting = new List<Pair>(initGridUnitInfo.setting);
        }
        clawState = ClawState.Close;
        HoldingUnits.Clear();

        this.transform.position = cell.transform.position;
        targetCellCache = null;
    }

    public void SaveToHistory()
    {
        Cell cellRef = cell;
        cellHistory.Push(cellRef);

        List<Pair> settingTemp = new List<Pair>(setting);
        settingHistory.Push(settingTemp);

        clawStateHistory.Push(clawState);

        List<GridUnit> holdingUnitsTemp = new List<GridUnit>(HoldingUnits);
        holdingUnitsHistory.Push(holdingUnitsTemp);
    }
    #endregion

    
}
