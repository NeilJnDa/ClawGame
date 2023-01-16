using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GridUnit
{
    public override UnitType unitType { get { return UnitType.Player; } }
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

    private void OnCheckInteraction()
    {
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


}
