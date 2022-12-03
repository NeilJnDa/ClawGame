using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

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
    public Event NextTurn;

    //Statistics
    [ReadOnly]
    [ShowInInspector]
    private int TotalTurnPassed = 0;
    [ReadOnly]
    [ShowInInspector]
    private int CurrentTurnUnitNumber = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
