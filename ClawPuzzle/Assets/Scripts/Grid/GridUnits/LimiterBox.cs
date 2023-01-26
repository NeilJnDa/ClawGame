using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;  

public class LimiterBox : PushableGridUnit
{
    public override UnitType unitType { get { return UnitType.LimiterBox; } }
    protected override void OnStart()
    {
        base.OnStart();
    }
    protected override void OnDelete()
    {
        base.OnDelete();
    }
}
