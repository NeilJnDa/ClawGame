using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimiterGround : GridUnit, ITurnUndo
{
    public override UnitType unitType { get { return UnitType.LimiterGround; } }
    public override bool pushable { get { return false; } }
    [SerializeField]
    private GameObject VFX;
    private void Start()
    {
        TurnManager.Instance.CheckInteractionEvent += CheckLimitationTarget;
    }


    private void OnDestroy()
    {
        TurnManager.Instance.CheckInteractionEvent -= CheckLimitationTarget;

    }

    private float CheckLimitationTarget()
    {
        SetVFX(ExistLimitationTarget());
        return 0;
    }

    private bool ExistLimitationTarget()
    {
        var cells = this.cell.grid.GetCellsFrom(this.cell, Direction.Above);
        foreach (var cell in cells)
        {
            foreach (var unit in cell.gridUnits)
            {
                if (unit.unitType == UnitType.Claw)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void SetVFX(bool b)
    {
        if (b)
        {
            VFX.transform.gameObject.SetActive(true);        }
        else
        {
            VFX.transform.gameObject.SetActive(false);
        }
    }
    #region ITurnUndo
    public void UndoOneStep()
    {
        SetVFX(ExistLimitationTarget());
    }

    public void ResetAll()
    {
        SetVFX(ExistLimitationTarget());
    }

    public void SaveToHistory()
    {

    }
    #endregion
}
