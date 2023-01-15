using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using System;
using static Controls;


public class InputManager : MonoBehaviour, IDefaultActions
{
    #region Singleton
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<InputManager>();
            }
            return _instance;
        }
    }
    private static InputManager _instance;
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
        controls = new Controls();
        controls.Default.SetCallbacks(this);
        EnableInput(InputEnabled);
    }

    #endregion
    private Controls controls;

    #region Enable Input or move
    //All input
    [field: SerializeField]
    public bool InputEnabled { get; private set; } = true;
    [InfoBox("Use Buttons to toggle", InfoMessageType.Warning)]
    [Button("Toggle Input Enabled"), HideInEditorMode]
    private void ToggleInputEnabled()
    {
        EnableInput(!InputEnabled);
    }

    //Only up, down, left, right, raise, drop
    [field: SerializeField]
    public bool MoveEnabled { get; private set; } = true;
    [Button("Toggle Move Enabled"), HideInEditorMode]
    private void ToggleMoveEnablde()
    {
        EnableMove(!MoveEnabled);
    }

    public void EnableInput(bool enable)
    {
        if (enable)
        {
            InputEnabled = true;
            controls.Default.Enable();
        }
        else
        {
            InputEnabled = false;
            controls.Default.Disable();
        }
    }
    public void EnableMove(bool enable)
    {
        MoveEnabled = enable;
    }
    #endregion


    #region Interface
    public event Action<Direction> moveEvent;
    public event Action clawActionEvent;
    public event Action undoEvent;
    public event Action resetEvent;

    public void OnDown(InputAction.CallbackContext context)
    {
        if (InputEnabled && MoveEnabled && context.performed)
        {
            Debug.Log("Down");
            moveEvent.Invoke(Direction.Down);
        }
    }


    #region Deprecated. Now claw action is binding to one key.
    //public void OnDrop(InputAction.CallbackContext context)
    //{
    //    if (InputEnabled && InputEnabled && MoveEnabled && context.performed)
    //    {
    //        Debug.Log("Drop");
    //        moveEvent.Invoke(Direction.Below);
    //    }
    //}
    //public void OnRaise(InputAction.CallbackContext context)
    //{
    //    if (InputEnabled && MoveEnabled && context.performed)
    //    {
    //        Debug.Log("Raise");
    //        moveEvent.Invoke(Direction.Above);
    //    }
    //}
    #endregion
    public void OnClaw(InputAction.CallbackContext context)
    {
        if (InputEnabled && MoveEnabled && context.performed)
        {
            Debug.Log("ClawAction");
            clawActionEvent.Invoke();
        }
    }
    public void OnSkip(InputAction.CallbackContext context)
    {
        if (InputEnabled && MoveEnabled && context.performed)
        {
            TurnManager.Instance.Skip();
        }
    }
    public void OnLeft(InputAction.CallbackContext context)
    {
        if (InputEnabled && MoveEnabled && context.performed)
        {
            Debug.Log("Left");
            moveEvent.Invoke(Direction.Left);
        }
    }



    public void OnRight(InputAction.CallbackContext context)
    {
        if (InputEnabled && MoveEnabled && context.performed)
        {
            Debug.Log("Right");
            moveEvent.Invoke(Direction.Right);
        }
    }
    public void OnUp(InputAction.CallbackContext context)
    {
        if (InputEnabled && MoveEnabled && context.performed)
        {
            Debug.Log("Up");
            moveEvent.Invoke(Direction.Up);
        }
    }
    public void OnUndo(InputAction.CallbackContext context)
    {
        if(InputEnabled && context.performed)
        {
            TurnManager.Instance.UndoOneStep();
        }
    }
    public void OnReset(InputAction.CallbackContext context)
    {
        if (InputEnabled && context.performed)
        {
            TurnManager.Instance.ResetAll();
        }
    }

    #endregion

}
