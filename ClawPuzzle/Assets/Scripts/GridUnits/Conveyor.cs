using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : GridUnit
{
    public override UnitType unitType { get { return UnitType.Conveyor; } }

    private void Start()
    {
        TurnManager.Instance.OnEnvTurnEvent += OnEnvTurn;
    }
    private void OnDestroy()
    {
        TurnManager.Instance.OnEnvTurnEvent -= OnEnvTurn;

    }

    private void OnEnvTurn()
    {
        Debug.Log(this.name + "EnvTurn");
    }
}
