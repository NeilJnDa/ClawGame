using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D 
{
    public Transform parentTransform { get; private set; }

    //Length: axis_x
    //Width: axis_z
    //height: axis_y
    public int length  {get; private set; }
    public int width { get; private set; }
    public int height { get; private set; }

    public Vector3 offset { get; private set; }

    public float size { get; private set; }
    public float spacing { get; private set; }
    public Cell [,,] cellMatrix { get; private set; }

    public Grid3D(Transform parentTransform, LevelData levelData)
    {
        this.parentTransform = parentTransform;
        GridSetting gridSetting = levelData.gridSetting;
        this.length = gridSetting.length;
        this.width = gridSetting.width;
        this.height = gridSetting.height;
        this.offset = gridSetting.offset; 
        this.size = gridSetting.size;
        this.spacing = gridSetting.spacing;

        cellMatrix = new Cell[length, width, height];

        for(int i = 0; i < length; ++i)
        {
            for (int j = 0; j < width; ++j)
            {  
                for(int k = 0; k < height; ++k)
                {
                    cellMatrix[i, j, k] = new Cell(i, j, k, this, levelData.GetUnit(i,j,k).unitType);
                    cellMatrix[i, j, k].Initialize();
                }
            }
        }
    }

}
