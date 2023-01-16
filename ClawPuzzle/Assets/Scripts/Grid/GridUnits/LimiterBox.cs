using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimiterBox : GridUnit
{
    public override UnitType unitType { get { return UnitType.LimiterBox; } }
    public override bool catchable { get { return true; } }
    public override bool pushable { get { return true; } }
    private void Start()
    {
        
        TurnManager.Instance.PlayerTurnEvent += OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent += OnEnvTurn;
        TurnManager.Instance.CheckInteractionEvent += OnCheckInteraction;


    }
    private void OnDestroy()
    {
        TurnManager.Instance.PlayerTurnEvent -= OnPlayerTurn;
        TurnManager.Instance.EnvTurnEvent -= OnEnvTurn;
        TurnManager.Instance.CheckInteractionEvent -= OnCheckInteraction;

    }


    private void OnPlayerTurn()
    {
        if (targetCellCache != null)
        {
            MoveToCell(targetCellCache, TurnManager.Instance.playerTurnDuration);
            targetCellCache = null;
        }
    }
    private void OnEnvTurn()
    {
        if (targetCellCache != null)
        {
            MoveToCell(targetCellCache, TurnManager.Instance.playerTurnDuration);
            targetCellCache = null;
        }
    }
    private void OnCheckInteraction()
    {
        var upperCells = this.cell.grid.GetCellsFrom(this.cell, Direction.Above);
        foreach (var cell in upperCells)
        {
            foreach (var unit in cell.gridUnits)
            {
                if (unit.unitType == UnitType.Claw)
                {
                    unit.OnLimitation();
                }
            }
        }
    }
}
