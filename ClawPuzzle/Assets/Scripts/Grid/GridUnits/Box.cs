using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class Box : PushableGridUnit
{
    public override UnitType unitType { get { return UnitType.Box; } }

    protected override void OnStart()
    {
        base.OnStart();
    }
    protected override void OnDelete()
    {
        base.OnDelete();
    }
   

}
