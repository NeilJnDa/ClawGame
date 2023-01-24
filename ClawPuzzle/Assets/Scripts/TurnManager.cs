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
    //Undo methods of each object are called from here
    //Events are called during a NextStepCoroutine() to broadcast
    public event Action StartStepProcessEvent;
    public event Action PlayerTurnEvent;
    public event Action EnvTurnEvent;
    public event Func<float> ClawOpenEvent;
    public event Func<float> ClawCloseEvent;
    public event Func<float> CheckInteractionEvent;
    public event Action EndStepProcessEvent;

    [ReadOnly]
    public int currentStep = 0;
    [ShowInInspector]
    [ReadOnly]
    public Stack<int> currentStepHistory = new Stack<int>();
    [ReadOnly]
    public int totalStep = 0;
    private IEnumerator NextStepInst = null;

    /// <summary>
    /// Skip One PlayerTurn
    /// </summary>
    public void Skip()
    {
        NextStep();
    }
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
        StartStepProcessEvent?.Invoke();

        //Disable more command
        InputManager.Instance.EnableMove(false);

        //Tell every unit to save their current state to the state history
        var units = InterfaceFinder.GetAllByInterface<ITurnUndo>();
        units.ForEach(x => x.SaveToHistory());
        currentStepHistory.Push(currentStep);
        currentStep++;
        totalStep++;

        //Open the claw if needed
        var timeOpen = ClawOpenEvent?.Invoke();
        yield return new WaitForSeconds((float)timeOpen);

        //Player Turn (anim/audio)
        PlayerTurnEvent?.Invoke();
        yield return new WaitForSeconds(playerTurnDuration);

        //Close the claw
        var timeClose = ClawCloseEvent?.Invoke();
        yield return new WaitForSeconds((float)timeClose);

        //After claw actions, check interactions (Limiter)
        var timeInteraction = CheckInteractionEvent?.Invoke();
        yield return new WaitForSeconds((float)timeInteraction);


        //Then Env Turn
        currentTurn = Turn.EnvTurn;
        //Tell every env units to execute
        EnvTurnEvent?.Invoke();

        yield return new WaitForSeconds(envTurnDuration);
        //After env actions, check interactions
        var timeInteraction2 = CheckInteractionEvent?.Invoke();
        yield return new WaitForSeconds((float)timeInteraction2);

        //TODO: 如果此时CheckInteraction造成有的unit要移动，额外回合？ （额外回合会不会造成更多的额外回合？
        //Accept command
        currentTurn = Turn.PlayerTurn;
        InputManager.Instance.EnableMove(true);

        EndStepProcessEvent?.Invoke();

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
        if (totalStep <= 0) return;
        currentStep = currentStepHistory.Pop();
        totalStep--;

        //Tell every unit to save their current state
        var units = InterfaceFinder.GetAllByInterface<ITurnUndo>();
        foreach (var unit in units)
        {
            unit.UndoOneStep();
        }

    }
    public void ResetAll()
    {
        StopNextTurnCoroutine();
        if (currentStep <= 0) return;
        totalStep++;
        currentStepHistory.Push(currentStep);
        currentStep = 0;
     
        //Tell every unit to save their current state
        var units = InterfaceFinder.GetAllByInterface<ITurnUndo>();
        foreach (var unit in units)
        {
            unit.ResetAll();
        }
    }

    #endregion

}
