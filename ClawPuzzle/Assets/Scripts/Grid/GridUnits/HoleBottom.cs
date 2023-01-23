using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleBottom : GridUnit
{
    public override UnitType unitType { get { return UnitType.HoleBottom; } }
    public override bool catchable { get { return false; } }
    public override bool pushable { get { return false; } }


}
