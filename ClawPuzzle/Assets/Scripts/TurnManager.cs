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
    public event Action PlayerTurnEvent;
    public event Action EnvTurnEvent;
    public event Action CheckInteractionEvent;

    [ReadOnly]
    public int currentStep = 0;
    private IEnumerator NextStepInst = null;
    /// <summary>
    /// Called When a plausible commanded is executed and consume a step
    /// </summary>
    public void NextStep()
    {
        NextStepInst = NextStepCoroutine() as IEnumerator;
        StartCoroutine(NextStepInst);
    }

    IEnumerator NextStepCoroutine()
    {
        //Disable more command
        InputManager.Instance.EnableMove(false);

        //Tell every unit to save their current state to the state history
        var units = InterfaceFinder.GetAllByInterface<ITurnUnit>();
        foreach (var unit in units)
        {
            unit.NextStep();
        }
        currentStep++;

        //First finish Player Turn (anim/audio)
        PlayerTurnEvent?.Invoke();
        yield return new WaitForSeconds(playerTurnDuration);
        //After claw actions, check interactions
        CheckInteractionEvent?.Invoke();

        //Then Env Turn
        currentTurn = Turn.EnvTurn;
        //Tell every env units to execute
        EnvTurnEvent?.Invoke();
        yield return new WaitForSeconds(envTurnDuration);
        //After env actions, check interactions
        CheckInteractionEvent?.Invoke();

        //Accept command
        currentTurn = Turn.PlayerTurn;
        InputManager.Instance.EnableMove(true);


    }
    private void StopNextTurnCoroutine()
    {
        if (NextStepInst != null)
        {
            StopCoroutine(NextStepInst);
            InputManager.Instance.EnableMove(true);
            if (currentTurn == Turn.PlayerTurn)
            {
                //TODO: Stop Player Anim
            }
            else if (currentTurn == Turn.EnvTurn)
            {
                //TODO: Stop env units anim
            }
            currentTurn = Turn.PlayerTurn;
        }
    }

    public void UndoOneStep()
    {
        StopNextTurnCoroutine();
        if (currentStep <= 0) return;
        currentStep--;

        //Tell every unit to save their current state
        var units = InterfaceFinder.GetAllByInterface<ITurnUnit>();
        foreach (var unit in units)
        {
            unit.UndoOneStep();
        }

    }
    public void ResetAll()
    {
        StopNextTurnCoroutine();
        if (currentStep <= 0) return;

        currentStep = 0;
     
        //Tell every unit to save their current state
        var units = InterfaceFinder.GetAllByInterface<ITurnUnit>();
        foreach (var unit in units)
        {
            unit.ResetAll();
        }
    }

    #endregion

}
