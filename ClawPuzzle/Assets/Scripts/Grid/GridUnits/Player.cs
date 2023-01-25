using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Player : GridUnit, ITurnUndo
{
    public override UnitType unitType { get { return UnitType.Player; } }
    public override bool pushable { get { return true; } }

    private void Start()
    {
        TurnManager.Instance.GravityEvent += OnGravity;
    }
    private void OnDestroy()
    {
        TurnManager.Instance.GravityEvent -= OnGravity;

    }
    private float OnGravity()
    {
        if (isCaught) return 0;

        if (CheckMoveAndPushToNext(this, this.cell, Direction.Below))
        {
            this.MoveToCell(cell.grid.GetClosestCell(this.cell, Direction.Below), TurnManager.Instance.gravityMoveEachDuration);
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
    public void UndoOneStep()
    {
        cell.Leave(this);
        cell = cellHistory.Pop();
        cell.Enter(this);
        setting = settingHistory.Pop();
        this.transform.DOKill();

        this.transform.position = cell.transform.position;
    }

    public void ResetAll()
    {

        Cell cellRef = cell;
        cellHistory.Push(cellRef);
        List<Pair> settingTemp = new List<Pair>(setting);
        settingHistory.Push(settingTemp);


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

    public void SaveToHistory()
    {
        Cell cellRef = cell;
        cellHistory.Push(cellRef);

        List<Pair> settingTemp = new List<Pair>(setting);
        settingHistory.Push(settingTemp);

    }
    #endregion

}
