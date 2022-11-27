using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/Level Data", order = 2)]
[InlineEditor]
public class LevelData : SerializedScriptableObject
{
    [OnValueChanged("CreateMatrix")]
    public GridSetting gridSetting;

    [TableMatrix(SquareCells = true)]
    [EnumPaging]
    public CellType[,] initCellMatrix;

    private void CreateMatrix()
    {
        if(gridSetting!= null)
        {
            if(initCellMatrix == null)
                //create new
                initCellMatrix = new CellType[gridSetting.length, gridSetting.width];
            else
            {
                //copy and resize
                var newMatrix = new CellType[gridSetting.length, gridSetting.width];
                for(int i = 0; i < initCellMatrix.GetLength(0); ++i)
                {
                    for(int j = 0; j < initCellMatrix.GetLength(1); ++j)
                    {
                        if(i < gridSetting.length && j < gridSetting.width)
                        {
                            newMatrix[i, j] = initCellMatrix[i, j];
                        }
                    }
                }
                //C# have auto GC
                initCellMatrix = newMatrix;
            }
        }
    }

}
