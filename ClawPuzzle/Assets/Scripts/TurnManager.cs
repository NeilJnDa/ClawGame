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

    //Event
    public event Action NextTurnEvent;
    public event Action UndoOneTurnEvent;
    public event Action ResetAllEvent;


    //Statistics
    [ReadOnly]
    [ShowInInspector]
    private int TotalTurnPassed = 0;
    [ReadOnly]
    [ShowInInspector]
    private int CurrentTurnUnitNumber = 0;

    public void Subscribe()
    {
        CurrentTurnUnitNumber++;
    }
    public void Unsubscribe()
    {
        CurrentTurnUnitNumber--;
        if (CurrentTurnUnitNumber < 0)
        {
            CurrentTurnUnitNumber = 0;

            Debug.LogError("Current Turn Unit Number < 0!");
        }
    }
}
