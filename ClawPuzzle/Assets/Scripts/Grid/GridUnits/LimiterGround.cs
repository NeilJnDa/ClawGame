using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimiterGround : GridUnit
{
    public override UnitType unitType { get { return UnitType.LimiterGround; } }
    public override bool catchable { get { return false; } }
    public override bool pushable { get { return false; } }
    private void Start()
    {
        TurnManager.Instance.CheckInteractionEvent += OnCheckInteraction;


    }


    private void OnDestroy()
    {
        TurnManager.Instance.CheckInteractionEvent -= OnCheckInteraction;


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
