using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : GridUnit
{
    public override UnitType unitType { get { return UnitType.Hole; } }
    public override bool catchable { get { return false; } }


}
