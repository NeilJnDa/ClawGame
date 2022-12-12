using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Claw : GridUnit
{
    public override UnitType unitType { get { return UnitType.Claw; } }

    private void Start()
    {
        InputManager.Instance.moveEvent += OnMove;
    }
    private void OnDestroy()
    {
        InputManager.Instance.moveEvent -= OnMove;
    }
    public void OnMove(Direction direction)
    {
        if (this.MoveTo(direction))
        {
            TurnManager.Instance.NextStep();
        }
    }


}
