using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : GridUnit
{
    public override UnitType unitType { get { return UnitType.Conveyor; } }
    public override bool pushable { get { return false; } }
    private void Start()
    {
        TurnManager.Instance.EnvTurnEvent += OnEnvTurn;
    }
    private void OnDestroy()
    {
        TurnManager.Instance.EnvTurnEvent -= OnEnvTurn;
    }

    private float OnEnvTurn()
    {
        Debug.Log(this.name + "EnvTurn");
        return 0; ;
    }
}
