using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class TurnManager : MonoBehaviour
{
  
    #region Singleton
    public static TurnManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TurnManager>();
            }
            return _instance;
        }
    }
    private static TurnManager _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion


    #region Turn
    public enum Turn
    {
        PlayerTurn,
        EnvTurn
    }
    [ReadOnly]
    public Turn currentTurn = Turn.PlayerTurn;
    [field: SerializeField]
    public float playerTurnDuration { get; private set; } = 0.5f;
    [field: SerializeField]
    public float envTurnDuration { get; private set; } = 0.5f;

    #endregion

    //Step: composed of two turns(player and env), one player action = one step
    #region
    //Event
    public event Action UndoOneStepEvent;
    public event Action ResetAllEvent;
    public event Action OnEnvTurnEvent;

    [ReadOnly]
    public int currentStep = 0;
    private IEnumerator NextStepInst = null;
    public void NextStep()
    {
        NextStepInst = NextStepCoroutine() as IEnumerator;
        StartCoroutine(NextStepInst);
    }

    IEnumerator NextStepCoroutine()
    {
        //First finish Player Turn (anim/audio)
        InputManager.Instance.EnableMove(false);
        yield return new WaitForSeconds(playerTurnDuration);

        //Then Env Turn
        currentTurn = Turn.EnvTurn;
        //Tell every env units to execute
        OnEnvTurnEvent?.Invoke();


        yield return new WaitForSeconds(envTurnDuration);

        currentStep++;
        currentTurn = Turn.PlayerTurn;
        InputManager.Instance.EnableMove(true);

    }

    #endregion

}
