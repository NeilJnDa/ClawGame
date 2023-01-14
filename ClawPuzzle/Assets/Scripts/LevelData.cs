using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[Serializable]
public struct GridUnitInfo
{
    public int i;
    public int j;
    public int k;

    public UnitType unitType;
    public List<Pair> setting;

    public GridUnitInfo(int i, int j, int k, UnitType unitType, List<Pair> setting)
    {
        this.i = i;
        this.j = j;
        this.k = k;
        this.unitType = unitType;
        if (setting != null)
            this.setting = setting;
        else this.setting = new List<Pair>();
    }

    public void Log()
    {
        Debug.Log("[GridUnitInfo] " + i + " " + j + " " + k + ", " + unitType + "; " + setting);
    }
}
[Serializable]
public struct CellInfo
{
    public int i;
    public int j;
    public int k;
    public bool[] solidSurface;

    public CellInfo(int i, int j, int k, bool[] solidSurface)
    {
        this.i = i;
        this.j = j;
        this.k = k;
        this.solidSurface = new bool[6] { false, false, false, false, false, false, };

        if(solidSurface != null)
        {
            solidSurface.CopyTo(this.solidSurface, 0);
        }
    }
    public void Log()
    {
        Debug.Log("[CellInfo] " + i + " " + j + " " + k + ", " + solidSurface);
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
    public List<GridUnitInfo> gridUnitInfos = new List<GridUnitInfo>();
    public List<CellInfo> cellInfos = new List<CellInfo>();

    public LevelData(string levelName, GridSetting gridSetting)
    {
        this.levelName = levelName;
        this.gridSetting = gridSetting;
    }
    public bool[] GetCellSolidSurface(int i, int j, int k)
    {
        foreach(var cellInfo in cellInfos)
        {
            if(cellInfo.i == i && cellInfo.j == j && cellInfo.k == k)
            {
                return cellInfo.solidSurface;
            }
        }
        return null;
    }
    public void SetCellSpace(int i, int j, int k, bool[] solidSurface)
    {
        if (i >= gridSetting.length || j >= gridSetting.width || k >= gridSetting.height)
        {
            Debug.LogError("Unit Index out of boundary. Check if the grid is to small or the unit is too far.");
        }
        cellInfos.Add(new CellInfo(i, j, k, solidSurface));
    }
    public void SetGridUnit(int i, int j, int k, UnitType unitType, List<Pair> setting)
    {
        if (i >= gridSetting.length || j >= gridSetting.width || k >= gridSetting.height)
        {
            Debug.LogError(unitType + ": Unit Index out of boundary. Check if the grid is to small or the unit is too far.");
        }
        gridUnitInfos.Add(new GridUnitInfo(i, j, k, unitType, setting));
    }
} 
