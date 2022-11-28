using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2D 
{
    public Transform parentTransform { get; private set; }
    public int length  {get; private set; }
    public int width { get; private set; }
    public float size { get; private set; }
    public float spacing { get; private set; }
    public Cell [,] cellArray { get; private set; }

    public Grid2D(LevelData levelData, Transform parentTransform)
    {
        this.parentTransform = parentTransform;
        GridSetting gridSetting = levelData.gridSetting;
        this.length = gridSetting.length;
        this.width = gridSetting.width;
        this.size = gridSetting.size;
        this.spacing = gridSetting.spacing;

        cellArray = new Cell[length, width];

        for(int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {  
                cellArray[i, j] = new Cell(i,j, this, levelData.initCellMatrix[i, j]);
                cellArray[i, j].Initialize();
            }
        }
    }

}
