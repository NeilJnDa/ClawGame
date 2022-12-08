using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public struct UnitInfo
{
    public int i;
    public int j;
    public int k;
    public UnitType unitType;
}
[Serializable]
public struct GridSetting
{
    [BoxGroup("Grid")]
    public int length;
    [BoxGroup("Grid")]
    public int width;
    [BoxGroup("Grid")]
    public int height;
    [BoxGroup("Grid")]
    [Tooltip("Distance from center of the gridbase to center of the first cell")]
    public Vector3 offset;
    [BoxGroup("Cell")]
    public float size;
    [BoxGroup("Cell")]
    public float spacing;
}
[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/Level Data", order = 2)]
[Serializable]
public class LevelData : ScriptableObject
{
    [TextArea]
    public string description;
    [InfoBox("@\"Init Cell Matrix Length: \" + (initCellMatrix != null ? initCellMatrix.Length : 0)")]
    public GridSetting gridSetting = new GridSetting { length = 8, width = 6, height = 5, offset = new Vector3(0.5f, 0.5f, 0.5f), size = 1, spacing = 0 };
    public UnitInfo[] initCellMatrix;

    public UnitInfo GetUnit(int i, int j, int k)
    {
        return initCellMatrix[i + j * gridSetting.length + k * gridSetting.length * gridSetting.width];
    }
    public UnitInfo GetUnit(Vector3Int pos)
    {
        return GetUnit(pos.x, pos.y, pos.z);
    }
    public void SetUnit(int i, int j, int k, UnitType unitType)
    {
        var index = i + j * gridSetting.length + k * gridSetting.length * gridSetting.width;
        initCellMatrix[index].i = i;
        initCellMatrix[index].j = j;
        initCellMatrix[index].k = k;
        initCellMatrix[index].unitType = unitType;
    }
    public void SetUnit(Vector3Int pos, UnitType unitType)
    {
        if(pos.x >= gridSetting.length || pos.y >= gridSetting.width || pos.z >= gridSetting.height)
        {
            Debug.LogError(unitType +  ": Unit Index out of boundary. Check if the grid is to small or the unit is too far.");
        }
        SetUnit(pos.x, pos.y, pos.z, unitType);
    }

} 
