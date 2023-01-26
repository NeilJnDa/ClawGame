using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : GridUnit
{
    public override UnitType unitType { get { return UnitType.Hole; } }
    public override bool pushable { get { return false; } }

    //private void Start()
    //{
    //    TurnManager.Instance.CheckInteractionEvent += CheckWin;
    //}
    //private float CheckPlayer()
    //{
    //    var upper = cell.NextCell(Direction.Above);
    //    if(upper && upper.gridUnits.Exists)
    //}
}
