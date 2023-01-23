using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// The virtual one-size space in the grid
/// </summary>
[System.Serializable]
public class Cell : MonoBehaviour
{
    //i: [0, length-1]
    //j: [0, width-1]
    //k: [0, height-1]

    [ReadOnly][ShowInInspector]
    public int i { get; private set; }
    [ReadOnly][ShowInInspector]
    public int j { get; private set; }
    [ReadOnly][ShowInInspector]
    public int k { get; private set; }

    public Grid3D grid { get; private set; } = null;
    public List<GridUnit> gridUnits = new List<GridUnit>();

    //For each direction, if it will block moving in
    [HorizontalGroup("Solid Surface", 0.5f, LabelWidth = 20)]
    [BoxGroup("Solid Surface/Direction")]
    [ReadOnly]
    [ShowInInspector]
    [System.NonSerialized]
    private Direction[] dirReference = new Direction[6] { Direction.Up, Direction.Down, Direction.Left, Direction.Right, Direction.Above, Direction.Below };
    [BoxGroup("Solid Surface/Has a glass")]
    public bool[] solidSurface = new bool[6];

    public void Initialize(int i, int j, int k, Grid3D grid, bool[] GetCellSolidSurface)
    {
        //Length pos_x : axis_x
        //Width pos_y : axis_z
        //height pos_z : axis_y
        this.i = i;
        this.j = j;
        this.k = k;
        this.grid = grid;
#if UNITY_EDITOR
        UnityEditor.SceneVisibilityManager.instance.DisablePicking(this.gameObject, false);
#endif
        //Glass
        GetCellSolidSurface?.CopyTo(this.solidSurface, 0);
        for (int index = 0; index < solidSurface.Length; ++index)
        {
            if (solidSurface[index])
            {
                var glass = GameObject.Instantiate(Resources.Load("Glass")) as GameObject;
                glass.transform.parent = this.transform;

                //The Glass prefab is initially placed facing Z axis
                if (dirReference[index] == Direction.Left || dirReference[index] == Direction.Right)
                {
                    glass.transform.rotation = Quaternion.Euler(0, 90f, 0);
                }
                if (dirReference[index] == Direction.Above || dirReference[index] == Direction.Below)
                {
                    glass.transform.rotation = Quaternion.Euler(90f, 0, 0);
                }
                Vector3 offset = DirectionToWorld(dirReference[index]) / 2f;
                glass.transform.position = transform.position + offset;
            }
        }
    }
    public void UpdatePosition(Transform parentTransform, GridSetting gridSetting)
    {
        i = Mathf.RoundToInt((this.transform.position.x - parentTransform.position.x - gridSetting.offset.x) / (gridSetting.size + gridSetting.spacing));
        j = Mathf.RoundToInt((this.transform.position.z - parentTransform.position.z - gridSetting.offset.z) / (gridSetting.size + gridSetting.spacing));
        k = Mathf.RoundToInt((this.transform.position.y - parentTransform.position.y - gridSetting.offset.y) / (gridSetting.size + gridSetting.spacing));
    }
    public void Enter(GridUnit newUnit)
    {
        gridUnits.Add(newUnit);
        newUnit.cell = this;
    }
    public void Leave(GridUnit unit)
    {
        gridUnits.Remove(unit);
    }

    #region HelperFunctions
    public Vector3 CellToWorld(Cell cell)
    {
        //Length i : axis_x
        //Width j : axis_z
        //height k : axis_y
        return cell.grid.parentTransform.position + cell.grid.offset +
            new Vector3(cell.i, cell.k, cell.j) * (cell.grid.spacing + cell.grid.size);
    }
    protected Vector3 DirectionToWorld(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.forward * (grid.size + grid.spacing);
            case Direction.Down:
                return Vector3.back * (grid.size + grid.spacing);
            case Direction.Left:
                return Vector3.left * (grid.size + grid.spacing);
            case Direction.Right:
                return Vector3.right * (grid.size + grid.spacing);
            case Direction.Above:
                return Vector3.up * (grid.size + grid.spacing);
            case Direction.Below:
                return Vector3.down * (grid.size + grid.spacing);
            default:
                return Vector3.zero;
        }
    }
    protected Vector3 DirectionToWorldNoSpacing(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3.forward * grid.size;
            case Direction.Down:
                return Vector3.back * grid.size;
            case Direction.Left:
                return Vector3.left * grid.size;
            case Direction.Right:
                return Vector3.right * grid.size;
            case Direction.Above:
                return Vector3.up * grid.size;
            case Direction.Below:
                return Vector3.down * grid.size;
            default:
                return Vector3.zero;
        }
    }

    #endregion

   
}

