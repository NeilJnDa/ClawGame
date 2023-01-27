using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;  

public class LimiterBox : PushableGridUnit, ITurnUndo
{
    public override UnitType unitType { get { return UnitType.LimiterBox; } }
    [SerializeField]
    private GameObject VFX;

    protected override void OnStart()
    {
        base.OnStart();
        TurnManager.Instance.CheckInteractionEvent += CheckLimitationTarget;

    }
    protected override void OnDelete()
    {
        base.OnDelete();
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
            VFX.transform.gameObject.SetActive(true);
        }
        else
        {
            VFX.transform.gameObject.SetActive(false);
        }
    }
    public override void UndoOneStep()
    {
        base.UndoOneStep();
        SetVFX(ExistLimitationTarget());
    }
    public override void ResetAll() { 
        base.UndoOneStep();
        SetVFX(ExistLimitationTarget());
    }

}
