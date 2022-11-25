using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/Level Data", order = 2)]
public class LevelData : ScriptableObject
{
    public GridSetting gridSetting;
    public CellType[,] initCellArray = new CellType[8,8];


}
