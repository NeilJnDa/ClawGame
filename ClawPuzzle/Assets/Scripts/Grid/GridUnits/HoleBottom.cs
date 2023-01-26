using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBottom : GridUnit
{
    public override UnitType unitType { get { return UnitType.HoleBottom; } }
    public override bool pushable { get { return false; } }

    private void Start()
    {
        TurnManager.Instance.EndStepProcessEvent += CheckPlayerWin;
    }

    private void CheckPlayerWin()
    {
        if(this.cell.gridUnits.Find(x=> x.unitType == UnitType.Player))
        {
            GameManager.Instance.CompleteLevel();
        }
    }
}
