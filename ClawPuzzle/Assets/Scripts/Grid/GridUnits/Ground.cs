using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : GridUnit
{
    public override UnitType unitType { get { return UnitType.Ground; } }
    public override bool pushable { get { return false; } }

}
