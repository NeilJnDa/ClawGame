using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Empty : GridUnit
{
    public override UnitType unitType { get { return UnitType.Empty; } }

    private void Awake()
    {
    #if UNITY_EDITOR
        UnityEditor.SceneVisibilityManager.instance.DisablePicking(this.gameObject, true);
     #endif
    }
}
