using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Grid Setting", menuName = "ScriptableObjects/Grid Setting", order = 1)]
public class GridSetting : ScriptableObject
{
    public int length = 8;
    public int width = 8;
    public float size = 4;
    public float spacing = 0;
}
