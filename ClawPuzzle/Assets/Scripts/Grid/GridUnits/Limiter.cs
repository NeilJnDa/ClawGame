using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limiter : GridUnit
{
    private void Start()
    {
        TurnManager.Instance.EnvTurnEvent += OnEnvTurn;
    }
    private void OnDestroy()
    {
        TurnManager.Instance.EnvTurnEvent -= OnEnvTurn;
    }

    private void OnEnvTurn()
    {
        Debug.Log(this.name + "EnvTurn");
    }
}
