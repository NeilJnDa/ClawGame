using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public struct CellInfo
{
    public int i;
    public int j;
    public int k;
    public bool[] solidSurface;

    //TODO: Assmue only one object
    public UnitType unitType;
    public Pair[] setting;
    public CellInfo(int i, int j, int k, UnitType unitType, bool[] solidSurface, Pair[] setting)
    {
        this.i = i;
        this.j = j;
        this.k = k;
        this.unitType = unitType;
        this.solidSurface = new bool[6] { false, false, false, false, false, false, };
        this.setting = new Pair[5];
        if (setting != null)
        {
            setting.CopyTo(this.setting, 0);
        }
        if(solidSurface != null)
        {
            solidSurface.CopyTo(this.solidSurface, 0);
        }
    }
    public void Log()
    {
        Debug.Log("[UnitInfo] " + i + " " + j + " " + k + ", " + unitType + ", " + setting + ", " + solidSurface);
    }
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

    public void Log()
    {
        Debug.Log("[Griding Setting] length: " + length + ", width: " + width + ", height: " + height +
            ", offset: " + offset + ", size: " + size + ", spacing: " + spacing);
    }
}

/// <summary>
/// Data class only for saving the level. Will be serialized. Json file will be desrialized to this and generate level accordingly.
/// Will not change during playing
/// </summary>
[Serializable]
public class LevelData
{
    public string levelName;
    public GridSetting gridSetting;
    public CellInfo[] initCellMatrix;

    public LevelData(string levelName, GridSetting gridSetting)
    {
        this.levelName = levelName;
        this.gridSetting = gridSetting;
        this.initCellMatrix = new CellInfo[gridSetting.length * gridSetting.width * gridSetting.height];
        for (int i = 0; i < gridSetting.length; ++i)
        {
            for (int j = 0; j < gridSetting.width; j++)
            {
                for (int k = 0; k < gridSetting.height; k++)
                {
                    var index = i + j * gridSetting.length + k * gridSetting.length * gridSetting.width;
                    initCellMatrix[index] = new CellInfo(i, j, k, UnitType.Empty, null, null);
                }
            }
        }
    }
    public CellInfo GetCell(int i, int j, int k)
    {
        return initCellMatrix[i + j * gridSetting.length + k * gridSetting.length * gridSetting.width];
    }
    public void SetCellSpace(int i, int j, int k, bool[] solidSurface)
    {
        if (i >= gridSetting.length || j >= gridSetting.width || k >= gridSetting.height)
        {
            Debug.LogError("Unit Index out of boundary. Check if the grid is to small or the unit is too far.");
        }
        var index = i + j * gridSetting.length + k * gridSetting.length * gridSetting.width;
        solidSurface.CopyTo(initCellMatrix[index].solidSurface, 0);
    }
    public void SetCellUnit(int i, int j, int k, UnitType unitType, Pair[] setting)
    {
        if (i >= gridSetting.length || j >= gridSetting.width || k >= gridSetting.height)
        {
            Debug.LogError(unitType + ": Unit Index out of boundary. Check if the grid is to small or the unit is too far.");
        }
        var index = i + j * gridSetting.length + k * gridSetting.length * gridSetting.width;
        initCellMatrix[index].unitType = unitType;
        setting.CopyTo(initCellMatrix[index].setting, 0);
    }
} 
