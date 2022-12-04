using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurnUnit : MonoBehaviour
{
    public struct state
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        TurnManager.Instance.NextTurnEvent += GoNextTurn;
        TurnManager.Instance.UndoOneTurnEvent += UndoOneTurn;
        TurnManager.Instance.ResetAllEvent += ResetAll;
    }
    void OnDestroy()
    {
        TurnManager.Instance.NextTurnEvent -= GoNextTurn;
        TurnManager.Instance.UndoOneTurnEvent -= UndoOneTurn;
        TurnManager.Instance.ResetAllEvent -= ResetAll;
    }
    protected virtual void GoNextTurn()
    {

    }
    protected virtual void UndoOneTurn()
    {

    }
    protected virtual void ResetAll()
    {

    }
}
