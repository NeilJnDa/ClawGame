using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Grid Setting", menuName = "ScriptableObjects/Grid Setting", order = 1)]
public class GridSetting : ScriptableObject
{
    [BoxGroup("Grid")]
    public int length = 8;
    [BoxGroup("Grid")]
    public int width = 8;
    [BoxGroup("Cell")]
    public float size = 4;
    [BoxGroup("Cell")]
    public float spacing = 0;
}
