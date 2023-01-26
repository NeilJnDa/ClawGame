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
        Waiting,
        PlayerTurn,
        PostPlayerTurn,
        EnvTurn,
        PostEnvTurn,
    }
    [ReadOnly]
    public Turn currentTurn = Turn.Waiting;
    [field: SerializeField]
    public float playerTurnMaxDuration { get; private set; } = 0.5f;
    [field: SerializeField]
    public float gravityMoveEachDuration { get; private set; } = 0.15f;


    #endregion

    //Step: composed of two turns(player and env), one player action = one step
    #region
    //Undo methods of each object are called from here
    //Events are called during a NextStepCoroutine() to broadcast
    public event Action StartStepProcessEvent;
    public event Func<float> PlayerTurnEvent;
    public event Func<float> EnvTurnEvent;
    public event Func<float> ClawOpenEvent;
    public event Func<float> ClawCloseEvent;
    public event Func<float> CheckInteractionEvent;
    public event Func<float> GravityEvent;
    public event Action EndStepProcessEvent;

    [ReadOnly]
    public int currentStep = 0;
    [ShowInInspector]
    [ReadOnly]
    public Stack<int> currentStepHistory = new Stack<int>();
    [ReadOnly]
    public int totalStep = 0;
    private IEnumerator NextStepInst = null;

    public void Initialize()
    {
        currentTurn = Turn.Waiting;
        currentStep = 0;
        currentStepHistory.Clear();
        totalStep = 0;
        NextStepInst = null;
    }
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
        currentTurn = Turn.PlayerTurn;
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
        var timeOpen = InvokeEventsReturnMax(ClawOpenEvent);
        //Debug.Log("TimeOpen " + timeOpen);
        yield return new WaitForSeconds((float)timeOpen);

        //Player Turn (anim/audio)
        var timePlayer = InvokeEventsReturnMax(PlayerTurnEvent);
        //Debug.Log("timePlayer " + timePlayer);

        yield return new WaitForSeconds((float)timePlayer);

        //Close the claw
        var timeClose = InvokeEventsReturnMax(ClawCloseEvent);
        //Debug.Log("timeClose " + timeClose);

        yield return new WaitForSeconds((float)timeClose);

        //After claw actions, check interactions (Limiter) and gravity
        currentTurn = Turn.PostPlayerTurn;
        var timeInteraction = InvokeEventsReturnMax(CheckInteractionEvent);

        float timeGravity = 0;
        float totalTimeGravity = 0;
        timeGravity = (float)InvokeEventsReturnMax(GravityEvent);

        while (timeGravity > 1e-5)
        {
            yield return new WaitForSeconds(timeGravity);
            totalTimeGravity += timeGravity;
            timeGravity = (float)InvokeEventsReturnMax(GravityEvent);
        }
        if(timeInteraction > totalTimeGravity) 
            yield return new WaitForSeconds(timeInteraction - totalTimeGravity);
        //Debug.Log("timeGravity " + timeGravity);
        //Debug.Log("timeInteraction " + timeInteraction);
        //Debug.Log("totalTimeGravity " + totalTimeGravity);



        //Then Env Turn
        currentTurn = Turn.EnvTurn;
        //Tell every env units to execute
        var timeEnv = InvokeEventsReturnMax(EnvTurnEvent);
        yield return new WaitForSeconds((float)timeEnv);

        if(timeEnv > 1e-5)
        {
            //PostEnv: Gravity and Limitation
            currentTurn = Turn.PostEnvTurn;
            var timeInteraction2 = InvokeEventsReturnMax(CheckInteractionEvent);
            float timeGravity2 = 0;
            float totalTimeGravity2 = 0;
            timeGravity2 = (float)InvokeEventsReturnMax(GravityEvent);

            while (timeGravity2 > 0)
            {
                yield return new WaitForSeconds(timeGravity2);
                totalTimeGravity2 += timeGravity2;
                timeGravity2 = (float)InvokeEventsReturnMax(GravityEvent);
            }
            if (timeInteraction2 > totalTimeGravity2)
                yield return new WaitForSeconds(timeInteraction2 - totalTimeGravity2);
        }


        //Accept command
        currentTurn = Turn.Waiting;
        InputManager.Instance.EnableMove(true);
        EndStepProcessEvent?.Invoke();
    }
    private void StopNextTurnCoroutine()
    {
        if (NextStepInst != null)
        {
            StopCoroutine(NextStepInst);
            InputManager.Instance.EnableMove(true);
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
    #region Helper Functions
    /// <summary>
    /// Invoke every subscriber of an event, and return that max returned values (min: 0)
    /// </summary>
    /// <param name="targetEvent"></param>
    /// <returns></returns>
    private float InvokeEventsReturnMax(Func<float> targetEvent)
    {
        if (targetEvent == null) return 0;
        float maxValue = 0;
        foreach(var eventHandler in targetEvent.GetInvocationList())
        {
            float value = (float)eventHandler.DynamicInvoke();
            if (value > maxValue) maxValue = value;

        }
        return maxValue;
    }
    #endregion
}
