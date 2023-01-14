using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Grid3D
{
    public Transform parentTransform { get; private set; }

    //Length: axis_x
    //Width: axis_z
    //height: axis_y
    public int length { get; private set; }
    public int width { get; private set; }
    public int height { get; private set; }

    public Vector3 offset { get; private set; }

    public float size { get; private set; }
    public float spacing { get; private set; }
    public Cell[,,] cellMatrix { get; private set; }

    public Grid3D(Transform parentTransform, LevelData levelData)
    {
        //if (levelData.initCellMatrix.Length == 0)
        //{
        //    Debug.LogError("No LevelData, check if parsing is completed or if the json has content.");
        //}
        levelData.gridSetting.Log();

        this.parentTransform = parentTransform;
        GridSetting gridSetting = levelData.gridSetting;
        this.length = gridSetting.length;
        this.width = gridSetting.width;
        this.height = gridSetting.height;
        this.offset = gridSetting.offset;
        this.size = gridSetting.size;
        this.spacing = gridSetting.spacing;

        cellMatrix = new Cell[length, width, height];
        for (int k = 0; k < height; ++k)
        {
            for (int i = 0; i < length; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    var cell = GameObject.Instantiate(Resources.Load("Cell", typeof(Cell)), this.parentTransform) as Cell;
                    if (cell == null) Debug.LogError("No Cell Created");
                    cell.transform.position = parentTransform.position + offset +
                        new Vector3(i * (size + spacing), k * (size + spacing), j * (size + spacing));
                    cell.gameObject.name = "Cell " + i + " " + j + " " + k;
                    cellMatrix[i, j, k] = cell;
                    cellMatrix[i, j, k].Initialize(i, j, k, this, levelData.GetCellSolidSurface(i, j, k));
                }
            }
        }
        Debug.Log(levelData.gridUnitInfos.Count);
        foreach(var gridUnitInfo in levelData.gridUnitInfos)
        {
            if (gridUnitInfo.unitType != UnitType.Empty)
            {
                gridUnitInfo.Log();
                var unit = GameObject.Instantiate(Resources.Load(gridUnitInfo.unitType.ToString(), typeof(GridUnit))) as GridUnit;
                unit.cell = cellMatrix[gridUnitInfo.i, gridUnitInfo.j, gridUnitInfo.k];
                if(gridUnitInfo.setting != null)
                    unit.setting = new List<Pair>(gridUnitInfo.setting);
                else unit.setting = new List<Pair>();

                unit.transform.position = unit.cell.transform.position;
                unit.transform.parent = unit.cell.transform;
                unit.cell.gridUnits.Add(unit);
            }
        }

    }
    public Cell GetClosestCell(Cell cell, Direction direction)
    {
        Vector3Int nextCellPos = new Vector3Int(cell.i, cell.j, cell.k);
        switch (direction)
        {
            case Direction.Up:
                nextCellPos.y += 1;
                break;
            case Direction.Down:
                nextCellPos.y -= 1;
                break;
            case Direction.Right:
                nextCellPos.x += 1;
                break;
            case Direction.Left:
                nextCellPos.x -= 1;
                break;
            case Direction.Above:
                nextCellPos.z += 1;
                break;
            case Direction.Below:
                nextCellPos.z -= 1;
                break;
        }
        if (!CheckPosValid(nextCellPos))
        {
            //Not Valid Cell Pos
            Debug.LogWarning(nextCellPos + " nextCellPos not valid");
            return null;
        }
        return GetCell(nextCellPos);
    }
    public Cell GetCell(Vector3Int pos)
    {
        return cellMatrix[pos.x, pos.y, pos.z];
    }
    public bool CheckPosValid(Vector3 pos)
    {
        if (pos.x < 0 || pos.x >= length)
            return false;
        if (pos.y < 0 || pos.y >= width)
            return false;
        if (pos.z < 0 || pos.z >= height)
            return false;
        return true;
    }

    public float AbsoluteDistance(Cell from, Cell to)
    {
        return Mathf.Abs(Vector3.Distance(new Vector3(from.i, from.j, from.k), new Vector3(to.i, to.j,to.k)));
    }
}
