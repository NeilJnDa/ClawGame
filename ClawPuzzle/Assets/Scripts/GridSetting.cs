using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Grid Setting", menuName = "ScriptableObjects/Grid Setting", order = 1)]
[System.Serializable]
public class GridSetting : ScriptableObject
{
    [BoxGroup("Grid")]
    public int length = 8;
    [BoxGroup("Grid")]
    public int width = 8;
    [BoxGroup("Grid")]
    public int height = 8;
    [BoxGroup("Grid")]
    [Tooltip("Distance from center of the gridbase to center of the first cell")]
    public Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);
    [BoxGroup("Cell")]
    public float size = 4;
    [BoxGroup("Cell")]
    public float spacing = 0;
}
