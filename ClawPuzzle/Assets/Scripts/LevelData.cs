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
    public Pair[] setting;

    public UnitInfo(int i, int j, int k, UnitType unitType, Pair[] setting)
    {
        this.i = i;
        this.j = j;
        this.k = k;
        this.unitType = unitType;
        if (setting == null)
            this.setting = new Pair[5];
        else
        {
            this.setting = new Pair[5];
            setting.CopyTo(this.setting, 0);
        }
        this.setting[0] = new Pair("Test Key", "Test Value");
    }
}
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
[Serializable]
public class LevelData
{
    public string levelName;
    public GridSetting gridSetting;
    public UnitInfo[] initCellMatrix;

    public LevelData(string levelName, GridSetting gridSetting)
    {
        this.levelName = levelName;
        this.gridSetting = gridSetting;
        this.initCellMatrix = new UnitInfo[gridSetting.length * gridSetting.width * gridSetting.height];

    }
    public UnitInfo GetUnit(int i, int j, int k)
    {
        return initCellMatrix[i + j * gridSetting.length + k * gridSetting.length * gridSetting.width];
    }
    public UnitInfo GetUnit(Vector3Int pos)
    {
        return GetUnit(pos.x, pos.y, pos.z);
    }
    public void SetUnit(int i, int j, int k, UnitType unitType, Pair[] setting)
    {
        var index = i + j * gridSetting.length + k * gridSetting.length * gridSetting.width;
        initCellMatrix[index] = new UnitInfo(i, j, k, unitType, setting);
    }
    public void SetUnit(Vector3Int pos, UnitType unitType, Pair[] setting)
    {
        if(pos.x >= gridSetting.length || pos.y >= gridSetting.width || pos.z >= gridSetting.height)
        {
            Debug.LogError(unitType +  ": Unit Index out of boundary. Check if the grid is to small or the unit is too far.");
        }
        SetUnit(pos.x, pos.y, pos.z, unitType, setting);
    }

} 
