using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : GridUnit
{
    public override UnitType unitType { get { return UnitType.Player; } }
    public override bool catchable { get { return true; } }


}
