using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using DG.Tweening;

public enum ClawState
{
    Open,
    Close,
    Catch
}
public class Claw : GridUnit, ITurnUndo
{
    [ReadOnly]
    public ClawState clawState;
    public override UnitType unitType { get { return UnitType.Claw; } }
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
    [Tooltip("When with an object, How many times speed of normal")]
    public float animationDuration = 0.5f;

    //Local Cahce
    private Animator animator;

    //A simple command cache. It will be set when player do something, and executed and cleared during player turn or env turn.
    public Cell targetCellCache = null;
    public bool willPush = false;
    public Direction moveDirectionCache;
    
    //Local flag for limitation
    private bool isUnderLimitation = false;

    private void Start()
    {
        InputManager.Instance.moveEvent += OnMoveInput;
        InputManager.Instance.clawActionEvent += OnClawActionInput;
        InputManager.Instance.liftEvent += OnLiftInput;

        TurnManager.Instance.PlayerTurnEvent += OnPlayerTurn;
        TurnManager.Instance.EndStepProcessEvent += OnEndStep;
        TurnManager.Instance.ClawOpenEvent += OnClawDroppingOpen;
        TurnManager.Instance.ClawCloseEvent += OnClawDroppingClose;
        TurnManager.Instance.CheckInteractionEvent += OnCheckInteraction;

        animator = GetComponentInChildren<Animator>();
        clawState = ClawState.Close;
    }


    private void OnDestroy()
    {
        InputManager.Instance.moveEvent -= OnMoveInput;
        InputManager.Instance.clawActionEvent -= OnClawActionInput;
        InputManager.Instance.liftEvent -= OnLiftInput;

        TurnManager.Instance.PlayerTurnEvent -= OnPlayerTurn;
        TurnManager.Instance.EndStepProcessEvent -= OnEndStep;
        TurnManager.Instance.ClawOpenEvent -= OnClawDroppingOpen;
        TurnManager.Instance.ClawCloseEvent -= OnClawDroppingClose;
        TurnManager.Instance.CheckInteractionEvent -= OnCheckInteraction;

    }
    #region Input Event Subscribers
    private void OnMoveInput(Direction direction)
    {
        Cell upperCell = this.cell.grid.GetClosestCell(this.cell, Direction.Above);
        List<Cell> stickCells = null;
        if(upperCell != null)
        {
            stickCells = upperCell.grid.GetCellsFrom(upperCell, Direction.Above);
        }
        Cell targetCell = this.cell.grid.GetClosestCell(this.cell, direction);

        
        bool success = true;
        if (clawState == ClawState.Close || clawState == ClawState.Catch)
        {
            //The claw Will Push
            success = success && this.CheckMoveAndPushToNext(this, this.cell, direction);
            willPush = true;
        }
        else if (clawState == ClawState.Open)
        {
            //The claw Will not push
            success = success && this.CheckMoveNoPush(this, this.cell, direction);
            willPush = false;
        }

        if (stickCells != null)
        {
            foreach (var stickCell in stickCells)
            {
                success = success && this.CheckMoveAndPushToNext(this, stickCell, direction);
            }
        }

        if (success)
        {
            targetCellCache = targetCell;
            moveDirectionCache = direction;
            TurnManager.Instance.NextStep();
        }
    }
    private void OnLiftInput()
    {
        if (isHolding)
        {
            if (CheckMoveAndPushToNext(this, this.cell, Direction.Above))
            {
                targetCellCache = this.cell.grid.GetClosestCell(this.cell, Direction.Above);
                TurnManager.Instance.NextStep();
            }
        }
        else
        {
            Cell targetCell = CheckMoveToEnd(Direction.Above);
            if(targetCell != null)
            {
                targetCellCache = targetCell;
                TurnManager.Instance.NextStep();
            }
        }

    }
    private void OnClawActionInput()
    {
        if (clawState == ClawState.Catch)
        {
            //TODO: 先SaveToHistory再Release
            if(TryRelease())
                TurnManager.Instance.NextStep();
        }
        else if(clawState == ClawState.Open || clawState == ClawState.Close)
        {
            Cell targetCell = CheckMoveToEnd(Direction.Below, true);
            //即使夹子没有移动空间，也会过一个回合
            targetCellCache = targetCell;
            isDropping = true;
            TurnManager.Instance.NextStep();
        }
    }

    #endregion
    #region Turn Event Subscribers
    private float OnPlayerTurn()
    {
        if(targetCellCache == null || targetCellCache == this.cell)
        {
            //No need to move
            targetCellCache = null;
            return 0;
        }

        //Calculate move duration
        float distance = cell.grid.AbsoluteDistance(this.cell, targetCellCache);
        float duration = TurnManager.Instance.playerTurnMaxDuration;
        if (isHolding)
            duration *= distance / holdingSpeedMultiplier;
        else
            duration *= distance / emptySpeedMultiplier;

        duration = Mathf.Clamp(duration, 0, TurnManager.Instance.playerTurnMaxDuration);

        if (willPush)
        {
            Cell next2Cell = targetCellCache.grid.GetClosestCell(targetCellCache, moveDirectionCache);
            if(next2Cell != null)
                PushIterator(targetCellCache, next2Cell, Direction.Right, duration);
        }
        //If push
        MoveToCell(targetCellCache, duration);

        //Move all Holding Units as well, No Rule Checking
        foreach (var unit in HoldingUnits)
        {
            unit.MoveToCell(targetCellCache, duration);
        }
        //Clear Cache
        targetCellCache = null;
        return duration;
    }
    private void PushIterator(Cell from, Cell to, Direction direction, float duration)
    {
        Debug.Log(from.name);
        Cell nextTwo = to.grid.GetClosestCell(to, direction);
        Debug.Log(nextTwo.name);
        Debug.Log(nextTwo.gridUnits.Count);

        if (nextTwo != null && from.gridUnits.Exists(x => x.pushable))
        {
            PushIterator(to, nextTwo, direction, duration);
        }
        foreach(var unit in from.gridUnits.ToArray())
        {
            Debug.Log(unit.name);

            if (unit.pushable)
                unit.MoveToCell(to, duration);
        }

    }
    private float OnCheckInteraction()
    {
        var limiter = CheckLimitation();
        if(limiter == null)
        {
            if (isUnderLimitation)
            {
                //From Limaitation to no limiation
                Debug.Log(this.name + " on End Limitation");
                isUnderLimitation = false;

                if (clawState == ClawState.Open)
                {
                    if (Catch()) return animationDuration;
                }
            }
        }

        else
        {
            if (!isUnderLimitation)
            {
                //From No Limaitation to Limiation
                Debug.Log(this.name + " on Limitation");
                isUnderLimitation = true;
                if (clawState == ClawState.Close || clawState == ClawState.Catch)
                {
                    if (TryRelease()) return animationDuration;
                }
                if (clawState == ClawState.Open)
                {
                    animator.SetTrigger("OpenToOpen");
                    return animationDuration;
                }
            }
        }
        return 0;
    }
    protected GridUnit CheckLimitation()
    {
        var cells = this.cell.grid.GetCellsFrom(this.cell, Direction.Below);
        foreach (var cell in cells)
        {
            foreach (var unit in cell.gridUnits)
            {
                if (unit.unitType == UnitType.LimiterBox || unit.unitType == UnitType.LimiterGround)
                {
                    return unit;
                }
            }
        }
        return null;
    }
    private float OnClawDroppingClose()
    {
        if (isDropping)
        {
            var limiter = CheckLimitation();
            if (limiter != null)
            {
                animator.SetTrigger("OpenToOpen");
                clawState = ClawState.Open;
                return animationDuration;
            }
            else
            {
                if(Catch())
                    return animationDuration;
            }
        }
        return 0;
    }


    private float OnClawDroppingOpen()
    {
        if (isDropping && clawState == ClawState.Close)
        {
            if (TryRelease()) return animationDuration;
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
        willPush = false;
        targetCellCache = null;
    }
    #endregion
    #region Behaviours
    /// <summary>
    /// From catch or close to open
    /// </summary>
    public bool TryRelease()
    {
        //Release claw
        if(clawState == ClawState.Close || clawState == ClawState.Catch)
        {
            clawState = ClawState.Open;
            animator.SetTrigger("ToOpen");

            foreach (var unit in HoldingUnits)
            {
                unit.isCaught = false;
            }
            HoldingUnits.Clear();
            Debug.Log(this.name + " Release Claw");
            return true;
        }
        return false;

    }
    public bool Catch()
    {
        if(clawState == ClawState.Catch || clawState == ClawState.Close)
        {
            return false;
        }
        else if (CheckAndCatchUnit())
        {
            Debug.Log("Claw Caught units");
            animator.SetTrigger("OpenToCatch");
            clawState = ClawState.Catch;
            return true;
        }
        else
        {
            Debug.Log("Claw Caught nothing but still close");
            animator.SetTrigger("OpenToClose");
            clawState = ClawState.Close;
            return true;
        }

    }
    private bool CheckAndCatchUnit()
    {
        var toCatchList = cell.gridUnits.FindAll(x => x.pushable == true);
        bool newCatch = false;
        foreach (var toCatch in toCatchList)
        {
            //Skip caught units 
            if (HoldingUnits.Contains(toCatch)) continue;
            HoldingUnits.Add(toCatch);
            toCatch.isCaught = true;
            Debug.Log(this.name + " catch " + toCatch.name);
            newCatch = true;
        }
        return newCatch;
    }
    /// <summary>
    /// Move this unit as far as possible in the desired direction. Return true and set targetCell if it can move at least one unit
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Cell CheckMoveToEnd(Direction direction, bool isClawToCatch = false)
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
                if (isClawToCatch && targetCell.gridUnits.Exists(x => x.pushable)) break;

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
            return targetCell;
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

        animator.Play(clawState.ToString(), 0);
        targetCellCache = null;
        this.transform.DOKill();
        this.transform.position = cell.transform.position;
    }

    public void ResetAll()
    {
        animator.ResetTrigger("OpenToClose");
        animator.ResetTrigger("OpenToOpen");
        animator.ResetTrigger("OpenToCatch");
        animator.ResetTrigger("ToOpen");
        animator.Play("Close", 0);
    
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
        this.transform.DOKill();
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
