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
    }


    private void OnDestroy()
    {
    }

}
