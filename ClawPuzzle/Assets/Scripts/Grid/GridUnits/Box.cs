using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Box : GridUnit, ITurnUndo
{
    public override UnitType unitType { get { return UnitType.Box; } }
    public override bool catchable { get { return true; } }
    public override bool pushable { get { return true; } }

    private void Start()
    {
        TurnManager.Instance.PlayerTurnEvent += OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent += OnEnvTurn;
        TurnManager.Instance.CheckInteractionEvent += OnCheckInteraction;
        TurnManager.Instance.EndStepProcessEvent += OnEndStep;
    }
    private void OnDestroy()
    {
        TurnManager.Instance.PlayerTurnEvent -= OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent -= OnEnvTurn;
        TurnManager.Instance.CheckInteractionEvent -= OnCheckInteraction;
        TurnManager.Instance.EndStepProcessEvent -= OnEndStep;
    }

    private void OnEndStep()
    {
    }

    private float OnCheckInteraction()
    {
        return 0;
    }

    private void OnEnvTurn()
    {
        if (targetCellCache != null)
        {
            MoveToCell(targetCellCache, TurnManager.Instance.playerTurnDuration);
            targetCellCache = null;
        }
    }

    private void OnPlayerTurn()
    {
        if(targetCellCache != null)
        {
            MoveToCell(targetCellCache, TurnManager.Instance.playerTurnDuration);
            targetCellCache = null;
        }
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

        targetCellCache = null;
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

        this.transform.position = cell.transform.position;
        targetCellCache = null;
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
