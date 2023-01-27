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
public enum ClawCommand
{
    Null,
    Release,
    Lift,
    Drop,
    Move
}
[Serializable]
public struct ClawCommandCache
{
    //A simple command cache. It will be set when player do something, and executed and cleared during player turn or env turn.
    public ClawCommand clawCommand;  //Default first one
    public Cell targetCell; //Default null
    public bool willPush;   //Default false
    public Direction direction; //Default first one
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

    [Tooltip("When empty How many times speed of normal")]
    public float emptySpeedMultiplier = 4f;
    [Tooltip("When with an object, How many times speed of normal")]
    public float holdingSpeedMultiplier = 1f;
    [Tooltip("When with an object, How many times speed of normal")]
    public float animationDuration = 0.5f;

    //Local Cahce
    private Animator animator;
    [SerializeField][ReadOnly]
    private ClawCommandCache clawCommandCache;
    
    
    //Local flag for limitation
    [ShowInInspector][ReadOnly]
    private bool isUnderLimitation = false;

    [Title("Audio")]
    public AudioClip clawOpenCloseSound;
    public AudioClip clawCatchSound;
    public AudioClip clawMoveSound;



    private void Start()
    {
        InputManager.Instance.moveEvent += OnMoveInput;
        InputManager.Instance.clawActionEvent += OnClawActionInput;
        InputManager.Instance.liftEvent += OnLiftInput;

        TurnManager.Instance.PlayerTurnEvent += OnPlayerTurn;
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
        TurnManager.Instance.ClawOpenEvent -= OnClawDroppingOpen;
        TurnManager.Instance.ClawCloseEvent -= OnClawDroppingClose;
        TurnManager.Instance.CheckInteractionEvent -= OnCheckInteraction;

    }
    #region Input Event Subscribers
    private void OnMoveInput(Direction direction)
    {
        clawCommandCache = new ClawCommandCache();
        Cell upperCell = this.cell.NextCell(Direction.Above);
        List<Cell> stickCells = null;
        if(upperCell != null)
        {
            stickCells = upperCell.grid.GetCellsFrom(upperCell, Direction.Above);
        }
        Cell targetCell = this.cell.NextCell(direction);

        
        bool success = true;
        if (clawState == ClawState.Close || clawState == ClawState.Catch)
        {
            //The claw Will Push
            success = success && this.CheckMoveAndPushToNext(this, this.cell, direction);
            clawCommandCache.willPush = true;
        }
        else if (clawState == ClawState.Open)
        {
            //The claw Will not push
            success = success && this.CheckMoveNoPush(this, this.cell, direction);
            clawCommandCache.willPush = false;
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
            clawCommandCache.targetCell = targetCell;
            clawCommandCache.direction = direction;
            clawCommandCache.clawCommand = ClawCommand.Move;
            TurnManager.Instance.NextStep();
        }
    }
    private void OnLiftInput()
    {
        clawCommandCache = new ClawCommandCache();
        if (isHolding)
        {
            if (CheckMoveAndPushToNext(this, this.cell, Direction.Above))
            {
                clawCommandCache.targetCell = this.cell.NextCell(Direction.Above);
                clawCommandCache.clawCommand = ClawCommand.Lift;
                clawCommandCache.direction = Direction.Above;
                TurnManager.Instance.NextStep();
            }
        }
        else
        {
            Cell targetCell = CheckMoveToEnd(Direction.Above);
            if(targetCell != null)
            {
                clawCommandCache.targetCell = targetCell;
                clawCommandCache.clawCommand = ClawCommand.Lift;
                clawCommandCache.direction = Direction.Above;
                TurnManager.Instance.NextStep();
            }
        }

    }
    private void OnClawActionInput()
    {
        clawCommandCache = new ClawCommandCache();

        if (clawState == ClawState.Catch)
        {
            clawCommandCache.clawCommand = ClawCommand.Release;
            TurnManager.Instance.NextStep();
        }
        else if(clawState == ClawState.Open || clawState == ClawState.Close)
        {
            if (ExistToCatchUnit())
            {
                //本格内有可夹的物品，直接夹并且过一回合。
            }
            else
            {
                Cell targetCell = CheckMoveToEnd(Direction.Below, true);
                clawCommandCache.targetCell = targetCell;
            }
            //即使夹子没有移动空间，也会过一个回合
            clawCommandCache.clawCommand = ClawCommand.Drop;
            clawCommandCache.direction = Direction.Below;
            TurnManager.Instance.NextStep();
        }
    }

    #endregion
    #region Turn Event Subscribers
    private float OnPlayerTurn()
    {
        if (clawCommandCache.clawCommand == ClawCommand.Release) TryRelease();
        if(clawCommandCache.targetCell == null || clawCommandCache.targetCell == this.cell)
        {
            //No need to move
            return 0;
        }

        //Calculate move duration
        float distance = cell.grid.AbsoluteDistance(this.cell, clawCommandCache.targetCell);
        float duration = TurnManager.Instance.playerTurnMaxDuration;
        if (isHolding)
            duration *= distance / holdingSpeedMultiplier;
        else
            duration *= distance / emptySpeedMultiplier;

        duration = Mathf.Clamp(duration, 0, TurnManager.Instance.playerTurnMaxDuration);

        if (clawCommandCache.willPush)
        {
            Cell next2Cell = clawCommandCache.targetCell.NextCell(clawCommandCache.direction);
            if (next2Cell != null)
                PushIterator(clawCommandCache.targetCell, next2Cell, clawCommandCache.direction, duration);
        }
        //The stick will always push
        List<Cell> upperCells = null;
        Cell upperOneCell = this.cell.NextCell(Direction.Above);
        if (upperOneCell) upperCells = upperOneCell.grid.GetCellsFrom(upperOneCell, Direction.Above);
        if (upperCells != null)
        {
            foreach (var upperCell in upperCells)
            {
                Cell upperTargetCell = upperCell?.NextCell(clawCommandCache.direction);
                Cell upperNext2Cell = upperTargetCell?.NextCell(clawCommandCache.direction);
                if (upperNext2Cell != null)
                    PushIterator(upperTargetCell, upperNext2Cell, clawCommandCache.direction, duration);
            }
        }

        //Move this
        MoveToCell(clawCommandCache.targetCell, clawCommandCache.direction, duration);
        AudioSource.PlayClipAtPoint(clawCatchSound, this.transform.position);

        //Move all Holding Units as well, No Rule Checking
        foreach (var unit in HoldingUnits)
        {
            unit.MoveToCell(clawCommandCache.targetCell, clawCommandCache.direction, duration);
        }
        return duration;
    }
    private void PushIterator(Cell from, Cell to, Direction direction, float duration)
    {
        Cell nextTwo = to.NextCell(direction);

        if (nextTwo != null && from.gridUnits.Exists(x => x.pushable))
        {
            PushIterator(to, nextTwo, direction, duration);
        }
        foreach(var unit in from.gridUnits.ToArray())
        {
            if (unit.pushable)
                unit.MoveToCell(to, direction, duration);
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
                GetComponentInChildren<ClawLineIndicator>().ChangeLineToWhite();

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
                GetComponentInChildren<ClawLineIndicator>().ChangeLineToRed();

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
        if (clawCommandCache.clawCommand == ClawCommand.Drop)
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
        if (clawCommandCache.clawCommand == ClawCommand.Drop && clawState == ClawState.Close)
        {
            if (TryRelease()) return animationDuration;
        }
        return 0;
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

            foreach (PushableGridUnit unit in HoldingUnits)
            {
                unit.isCaught = false;
            }
            HoldingUnits.Clear();
            AudioSource.PlayClipAtPoint(clawOpenCloseSound, this.transform.position);
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
        else if (TryCatchUnit())
        {
            Debug.Log("Claw Caught units");
            animator.SetTrigger("OpenToCatch");
            AudioSource.PlayClipAtPoint(clawOpenCloseSound, this.transform.position);
            clawState = ClawState.Catch;
            return true;
        }
        else
        {
            Debug.Log("Claw Caught nothing but still close");
            animator.SetTrigger("OpenToClose");
            AudioSource.PlayClipAtPoint(clawOpenCloseSound, this.transform.position);
            clawState = ClawState.Close;
            return true;
        }

    }
    /// <summary>
    /// Execute in the CheckInteraction event or ClawDroppingClose Event
    /// </summary>
    /// <returns></returns>
    private bool TryCatchUnit()
    {
        var toCatchList = cell.gridUnits.FindAll(x => x.pushable == true);
        bool newCatch = false;
        foreach (PushableGridUnit toCatch in toCatchList)
        {
            //Skip caught units 
            if (HoldingUnits.Contains(toCatch)) continue;
            HoldingUnits.Add(toCatch);
            toCatch.isCaught = true;
            Debug.Log(this.name + " catch " + toCatch.name);
            newCatch = true;
            AudioSource.PlayClipAtPoint(clawCatchSound, this.transform.position);
        }
        return newCatch;
    }
    /// <summary>
    /// Check if this cell has units to catch
    /// </summary>
    /// <returns></returns>
    private bool ExistToCatchUnit()
    {
        var toCatchList = cell.gridUnits.FindAll(x => x.pushable == true);
        bool newCatch = false;
        foreach (PushableGridUnit toCatch in toCatchList)
        {
            //Skip caught units 
            if (HoldingUnits.Contains(toCatch)) continue;
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
            targetCell = currentCell.NextCell(direction);
            if (targetCell != null && Rules.Instance.CheckEnterCell(this, currentCell, targetCell, direction, isClawToCatch))
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
        isUnderLimitation = CheckLimitation() != null ? true : false;

        animator.Play(clawState.ToString(), 0);
        clawCommandCache = default(ClawCommandCache);

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
        isUnderLimitation = CheckLimitation() != null ? true : false;

        this.transform.DOKill();
        this.transform.position = cell.transform.position;
        clawCommandCache = default(ClawCommandCache);
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
