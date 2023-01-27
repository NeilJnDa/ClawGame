using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class PushableGridUnit : GridUnit, ITurnUndo
{
    public override UnitType unitType { get { return UnitType.Empty; } }
    public override bool pushable { get { return true; } }
    [ReadOnly]
    public bool isCaught = false;

    private void Start()
    {
        OnStart();
    }
    private void OnDestroy()
    {
        OnDelete();
    }
    protected virtual void OnStart()
    {
        TurnManager.Instance.GravityEvent += OnGravity;

    }
    protected virtual void OnDelete()
    {
        TurnManager.Instance.GravityEvent -= OnGravity;
    }
    private float OnGravity()
    {
        if (isCaught) return 0;

        if (CheckMoveAndPushToNext(this, this.cell, Direction.Below))
        {
            this.MoveToCell(cell.NextCell(Direction.Below), Direction.Below, TurnManager.Instance.gravityMoveEachDuration);
            return TurnManager.Instance.gravityMoveEachDuration;
        }
        return 0;
    }

    #region ITurnUndo
    [ShowInInspector]
    [ReadOnly]
    Stack<Cell> cellHistory = new Stack<Cell>();
    [ShowInInspector]
    [ReadOnly]
    Stack<List<Pair>> settingHistory = new Stack<List<Pair>>();
    [ShowInInspector]
    [ReadOnly]
    Stack<bool> isCaughtHistory = new Stack<bool>();
    public virtual void UndoOneStep()
    {
        cell.Leave(this);
        cell = cellHistory.Pop();
        cell.Enter(this);
        setting = settingHistory.Pop();
        isCaught = isCaughtHistory.Pop();
        this.transform.DOKill();

        this.transform.position = cell.transform.position;
    }

    public virtual void ResetAll()
    {

        Cell cellRef = cell;
        cellHistory.Push(cellRef);
        List<Pair> settingTemp = new List<Pair>(setting);
        settingHistory.Push(settingTemp);
        isCaughtHistory.Push(false);

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
        this.transform.DOKill();

        this.transform.position = cell.transform.position;
    }

    public virtual void SaveToHistory()
    {
        Cell cellRef = cell;
        cellHistory.Push(cellRef);

        List<Pair> settingTemp = new List<Pair>(setting);
        settingHistory.Push(settingTemp);

        isCaughtHistory.Push(isCaught);

    }
    #endregion

}
