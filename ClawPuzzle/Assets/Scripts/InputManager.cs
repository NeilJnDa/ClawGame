using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;
using static Controls;

public class InputManager : MonoBehaviour, IDefaultActions
{
    private Controls controls;
    private void Awake()
    {
        controls = new Controls();
        controls.Default.SetCallbacks(this);
        EnableInput(InputEnabled);
    }
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
    public void OnDown(InputAction.CallbackContext context)
    {
        if(MoveEnabled)
            Debug.Log("Down");
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (MoveEnabled)
            Debug.Log("Drop");
    }

    public void OnLeft(InputAction.CallbackContext context)
    {
        if (MoveEnabled)
            Debug.Log("Left");
    }

    public void OnRaise(InputAction.CallbackContext context)
    {
        if (MoveEnabled)
            Debug.Log("Raise");
    }



    public void OnRight(InputAction.CallbackContext context)
    {
        if (MoveEnabled)
            Debug.Log("Right");
    }
    public void OnUp(InputAction.CallbackContext context)
    {
        if (MoveEnabled)
            Debug.Log("Up");
    }
    public void OnUndo(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    public void OnReset(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    #endregion

}
