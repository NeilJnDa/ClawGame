using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimiterBox : Limiter
{
    public override UnitType unitType { get { return UnitType.LimiterBox; } }
    public override bool catchable { get { return true; } }
    public override bool pushable { get { return true; } }

}
